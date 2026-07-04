using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly TmsDbContext _context;

    public TestController(TmsDbContext context)
    {
        _context = context;
    }

    [HttpGet("deferred")]
    public IActionResult Deferred()
    {
        var query = _context.Students.Where(s => s.GPA >= 3.0m);

        var orderedQuery = query.OrderBy(s => s.Name);

        var results = orderedQuery.ToList();

        return Ok(results);
    }
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        var totalStudents = _context.Students.Count();
        var activeStudents = _context.Students.Count(s => s.IsActive);
        var avgGpa = _context.Students.Average(s => s.GPA);

        return Ok(new
        {
            TotalStudents = totalStudents,
            ActiveStudents = activeStudents,
            AverageGPA = avgGpa
        });
    }

    [HttpGet("students-with-courses")]
    public IActionResult GetStudentsWithCourses()
    {
        var data = _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .ToList();

        return Ok(data);
    }
    [HttpGet("course-count")]
    public IActionResult CourseStudentCount()
    {
        var result = _context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                Course = g.Key,
                Students = g.Count()
            })
            .ToList();

        return Ok(result);
    }
    // Helper method (EF Core cannot translate this into SQL)
    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }

    [HttpGet("translation-fail")]
    public IActionResult TranslationFail()
    {
        Console.WriteLine("Running non-translatable query...");

        try
        {
            var students = _context.Students
      .AsEnumerable()
      .Where(s => IsHonorRoll(s.GPA))
      .ToList();

            return Ok(students);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }
}