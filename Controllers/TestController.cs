namespace TmsApi.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TmsApi.Infrastructure.Data;
using TmsApi.Infrastructure.Persistence;


[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly TmsDbContext _context;

    public TestController(TmsDbContext context)
    {
        _context = context;
    }

    // All your other endpoints...

    [HttpGet("nplusone")]
    public async Task<IActionResult> NPlusOne(
        CancellationToken cancellationToken = default)
    {
        var students = await _context.Students
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var s in students)
        {
            var count = await _context.Enrollments
                .AsNoTracking()
                .CountAsync(e => e.StudentId == s.Id, cancellationToken);

            Console.WriteLine($"{s.Name}: {count} enrollments");
        }

        return Ok("Check the SQL log in the console.");
    }

    [HttpGet("nplusone-fixed")]
    public async Task<IActionResult> NPlusOneFixed(
        CancellationToken cancellationToken = default)
    {
        var report = await _context.Students
            .AsNoTracking()
            .Select(s => new
            {
                s.Name,
                EnrollmentCount = s.Enrollments.Count()
            })
            .ToListAsync(cancellationToken);

        return Ok(report);
    }
    [HttpDelete("student/{id}")]
public async Task<IActionResult> SoftDeleteStudent(int id)
{
    var student = await _context.Students.FindAsync(id);

    if (student == null)
        return NotFound();

    student.IsDeleted = true;

    await _context.SaveChangesAsync();

    return NoContent();
}
[HttpPut("deactivate-low-gpa")]
public async Task<IActionResult> DeactivateLowGpaStudents()
{
    var rowsAffected = await _context.Students
        .Where(s => s.GPA < 2.0m)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(s => s.IsActive, false));

    return Ok(new
    {
        Message = "Students updated successfully.",
        RowsAffected = rowsAffected
    });
}
}