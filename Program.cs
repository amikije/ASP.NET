using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TmsApi.Api.Filters;
using TmsApi.Application.Services;
using TmsApi.Infrastructure.Data;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging());

// Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});

// Problem Details
builder.Services.AddProblemDetails();

// OpenAPI
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddScoped<ICourseService, CourseService>();

// TODO: Register EnrollmentService after its implementation is created.
// builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// Validate DI
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var app = builder.Build();
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
// Apply migrations and seed database
var connectionString = builder.Configuration.GetConnectionString("TmsDatabase");

Console.WriteLine($"Connection String = '{connectionString}'");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    context.Database.Migrate();

    DatabaseSeeder.Seed(context);
}

// Error handling
app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapOpenApi();
app.MapScalarApiReference();

// app.UseHttpsRedirection();

app.MapControllers();

app.Run();