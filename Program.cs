using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EnrollmentLab.Services;
using EnrollmentLab.Options;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Controllers;
using TmsApi.Services;
using Tms.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<TmsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging());

// Authentication
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training",
        options => { });

// Authorization
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// Problem Details
builder.Services.AddProblemDetails();

// OpenAPI
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

// Validate DI
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

// Options Pattern
builder.Services
    .AddOptions<AssessmentOptions>()
    .Bind(builder.Configuration.GetSection("Assessment"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddControllers(options =>
{
options.Filters.Add<AuditLogFilter>();
});
builder.Services.AddScoped<ICourseService, CourseService>();
var app = builder.Build();

// Apply Migrations and Seed Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    context.Database.Migrate();

    DatabaseSeeder.Seed(context);
}

// Error Handling
app.UseExceptionHandler();
app.UseStatusCodePages();

// Development Tools
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Sample Protected Endpoint
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
}).RequireAuthorization();

// Test ProblemDetails Endpoint
app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException(
        "Simulated database failure for ProblemDetails testing");
});

app.MapControllers();

app.Run();