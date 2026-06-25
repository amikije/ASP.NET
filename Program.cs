using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EnrollmentLab.Services;
using EnrollmentLab.Options;

var builder = WebApplication.CreateBuilder(args);

// =====================================
// Authentication
// =====================================

builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training",
        options => { });

builder.Services.AddAuthorization();

// =====================================
// Controllers
// =====================================

builder.Services.AddControllers();

// =====================================
// Swagger (Exercise 5)
// =====================================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================================
// Dependency Injection
// =====================================

builder.Services.AddScoped<
    IEnrollmentService,
    EnrollmentService>();

builder.Services.AddSingleton<EnrollmentWorker>();

// =====================================
// Validate Dependency Injection
// =====================================

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

// =====================================
// Options Pattern
// Reads Assessment section from
// appsettings.json
// =====================================

builder.Services
    .AddOptions<AssessmentOptions>()
    .Bind(builder.Configuration.GetSection("Assessment"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

// =====================================
// Swagger Middleware
// =====================================

app.UseSwagger();
app.UseSwaggerUI();

// =====================================
// Middleware Pipeline
// =====================================

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// =====================================
// Protected Endpoint
// =====================================

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

// =====================================
// Controller Routes
// =====================================

app.MapControllers();

app.Run();