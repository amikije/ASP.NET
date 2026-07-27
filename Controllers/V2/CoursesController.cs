using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Interfaces;
using TmsApi.Infrastructure.Services;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/courses")]
[ApiVersion("2.0")]
public class CoursesController : ControllerBase
{
    private readonly ICachedCourseService _cachedCourseService;

    public CoursesController(ICachedCourseService cachedCourseService)
    {
        _cachedCourseService = cachedCourseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var allCourses = await _cachedCourseService.GetAllCoursesAsync(ct);

        var totalCount = allCourses.Count;
        var rows = allCourses
            .OrderBy(c => c.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var hasNext = page < totalPages;
        var hasPrevious = page > 1;

        return Ok(new
        {
            data = rows.Select(c => new
            {
                c.Id,
                c.Title,
                c.Code,
                c.MaxCapacity,
                EnrollmentCount = c.EnrollmentCount
            }),
            meta = new
            {
                totalCount,
                page,
                pageSize,
                totalPages,
                hasNext,
                hasPrevious
            },
            links = new
            {
                self = $"/api/v2/courses?page={page}&pageSize={pageSize}",
                next = hasNext ? $"/api/v2/courses?page={page + 1}&pageSize={pageSize}" : (string?)null,
                prev = hasPrevious ? $"/api/v2/courses?page={page - 1}&pageSize={pageSize}" : (string?)null,
                enroll = "/api/v2/enrollments"
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(int id, CancellationToken ct)
    {
        // Note: For detail view, you might want to cache individual courses too
        // For now, we'll get all and filter (or you can add GetCourseById to the cache service)
        var allCourses = await _cachedCourseService.GetAllCoursesAsync(ct);
        var course = allCourses.FirstOrDefault(c => c.Id == id);

        if (course is null)
            return NotFound();

        return Ok(new
        {
            data = new
            {
                course.Id,
                course.Code,
                course.Title,
                course.MaxCapacity,
                EnrollmentCount = course.EnrollmentCount
            },
            links = new
            {
                self = $"/api/v2/courses/{id}",
                enroll = "/api/v2/enrollments"
            }
        });
    }
}