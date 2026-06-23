using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EnrollmentLab.Services;
using EnrollmentLab.Options;
var builder = WebApplication.CreateBuilder(args);

// Authentication
builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training", options => { });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Exercise 2 registrations
builder.Services.AddScoped<
    IEnrollmentService,
    EnrollmentService>();

builder.Services.AddSingleton<EnrollmentWorker>();

// Enable validation
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
//  fetch from appsettings.json
 builder.Services
    .AddOptions<AssessmentOptions>()
    .Bind(builder.Configuration.GetSection("Assessment"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

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

app.MapControllers();

app.Run();