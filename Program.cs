using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TmsApi.Api.Filters;
using TmsApi.Application.Services;
using TmsApi.Infrastructure.Data;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Infrastructure.Services;
using MediatR;
using TmsApi.Application;
using Asp.Versioning;
using TmsApi.Api.Middleware;
using TmsApi.Api.ExceptionHandlers;
using FluentValidation;
using TmsApi.Application.Behaviors;
using TmsApi.Application.Enrollments.Commands;
using TmsApi.Application.Enrollments.Queries;
using TmsApi.Application.Interfaces;
using TmsApi.Infrastructure.Repositories;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using TmsApi.Api.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. CONFIGURE SERVICES
// ============================================================

// ---------- Database Configuration ----------
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging());

// ---------- Controllers ----------
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});

// ---------- API Versioning ----------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ---------- CQRS with MediatR ----------
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(TmsApi.Application.AssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(EnrollStudentHandler).Assembly);
});

// ---------- FluentValidation ----------
builder.Services.AddValidatorsFromAssembly(typeof(EnrollStudentValidator).Assembly);

// ---------- Pipeline Behaviors ----------
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ---------- Exception Handling ----------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ---------- Caching ----------
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2)
    };
});

// ---------- Rate Limiting ----------
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var (partitionKey, tier) = ApiKeyResolver.Resolve(httpContext);

        return tier switch
        {
            ApiKeyTier.Paid => RateLimitPartition.GetTokenBucketLimiter(
                $"paid:{partitionKey}",
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 200,
                    TokensPerPeriod = 100,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }),

            ApiKeyTier.Free => RateLimitPartition.GetTokenBucketLimiter(
                $"free:{partitionKey}",
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 30,
                    TokensPerPeriod = 10,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }),

            _ => RateLimitPartition.GetTokenBucketLimiter(
                $"anon:{partitionKey}",
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                })
        };
    });

    options.AddTokenBucketLimiter("search", opt =>
    {
        opt.TokenLimit = 10;
        opt.TokensPerPeriod = 5;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 2;
        opt.AutoReplenishment = true;
    });

    options.AddConcurrencyLimiter("transcripts", opt =>
    {
        opt.PermitLimit = 5;
        opt.QueueLimit = 20;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, ct) =>
    {
        var retryAfter = "10";
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ts))
            retryAfter = ((int)ts.TotalSeconds).ToString();

        context.HttpContext.Response.Headers.RetryAfter = retryAfter;
        context.HttpContext.Response.ContentType = "application/problem+json";

        await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "Rate limit exceeded",
            Detail = $"Too many requests. Retry after {retryAfter} seconds.",
            Status = StatusCodes.Status429TooManyRequests,
            Type = "https://tms.local/errors/rate_limit_exceeded"
        }, ct);
    };
});

// ---------- OpenAPI / Scalar ----------
builder.Services.AddOpenApi();

// ---------- Dependency Injection ----------
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICachedCourseService, CachedCourseService>();

// ---------- Validation for DI ----------
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

// ============================================================
// 2. BUILD APPLICATION
// ============================================================

var app = builder.Build();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

// ============================================================
// 3. DATABASE MIGRATION & SEEDING
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context);
}

// ============================================================
// 4. MIDDLEWARE PIPELINE
// ============================================================

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseMiddleware<V1DeprecationMiddleware>();
app.UseRateLimiter();

// ---------- OpenAPI & Scalar ----------
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("TMS API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    // Remove obsolete methods:
    // .WithPreferredScheme("Bearer")  // OBSOLETE - remove
    // .WithDarkModeToggle(true)       // OBSOLETE - remove or use HideDarkModeToggle()
});

app.MapControllers();

// ============================================================
// 5. RUN APPLICATION
// ============================================================

app.Run();