using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EnrollmentLab.Services;
using EnrollmentLab.Options;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// ProblemDetails
builder.Services.AddProblemDetails();

// OpenAPI (required for Scalar)
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddSingleton<
    IEnrollmentService,
    EnrollmentService>();

builder.Services.AddSingleton<EnrollmentWorker>();

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

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();

    context.Database.Migrate();

    DatabaseSeeder.Seed(context);
}

// Exercise 7: Environment Toggle


if (app.Environment.IsDevelopment())
{
    // OpenAPI document
    app.MapOpenApi();

    // Scalar API Explorer
    app.MapScalarApiReference();
}
else
{
    // Production error handling
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// Protected Endpoint
app.MapGet("/api/assessments/results", () =>
{
    return Results.Ok(new
    {
        courseCode = "CS-101",
        studentId = "S-001",
        letterGrade = "A"
    });
})
.RequireAuthorization();

// Test Error Endpoint
app.MapGet("/api/error", () =>
{
    throw new TmsDatabaseException(
        "Simulated database failure for ProblemDetails testing");
});

app.MapControllers();

app.Run();