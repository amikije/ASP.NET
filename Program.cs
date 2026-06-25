using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EnrollmentLab.Services;
using EnrollmentLab.Options;

var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training",
        options => { });

builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// ProblemDetails (Exercise 6)
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================
// Dependency Injection
// =====================================

// IMPORTANT:
// Singleton keeps one EnrollmentService
// instance alive for the whole application.
//
// This allows the in-memory Dictionary
// to keep enrollment records between
// POST, GET and DELETE requests.

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

// Exception Handling
app.UseExceptionHandler();
app.UseStatusCodePages();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

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