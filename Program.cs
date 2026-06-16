using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training", options => { });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

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