using Microsoft.AspNetCore.Mvc;
using Tms.Api.Dtos;
using TmsApi.Dtos.Course;
using TmsApi.Services;

namespace TmsApi.Controllers;

[ApiController]
[Route("api/courses")]
[Tags("Courses")]
[Produces("application/json")]
public class CoursesController(
    ICourseService courseService,
    LinkGenerator linkGenerator)
    : ControllerBase
{
    [HttpGet("{id:int}", Name = nameof(GetCourseById))]
    [EndpointSummary("Get a course by ID")]
    [EndpointDescription("Returns a course together with HATEOAS links.")]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseById(
        int id,
        CancellationToken ct)
    {
        var course = await courseService.GetByIdAsync(id, ct);

        if (course is null)
            return NotFound();

        // Build links
        var coursePath = linkGenerator.GetPathByName(
            HttpContext,
            nameof(GetCourseById),
            new { id });

        var enrollmentsPath = linkGenerator.GetPathByAction(
            HttpContext,
            action: "GetEnrollments",
            controller: "Enrollments",
            values: new { courseId = id });

        // HATEOAS links
        var links = new List<LinkDto>
        {
            new LinkDto(coursePath!, "self", "GET"),
            new LinkDto(coursePath!, "update", "PUT"),
            new LinkDto(coursePath!, "delete", "DELETE"),
            new LinkDto(enrollmentsPath!, "enrollments", "GET")
        };

        // Only show the enroll link if the course has space
        if (course.EnrollmentCount < course.MaxCapacity)
        {
            links.Add(
                new LinkDto(
                    enrollmentsPath!,
                    "enroll",
                    "POST"));
        }

        // Build the response
        var detailDto = new CourseDetailDto(
            course.Id,
            course.Code,
            course.Title,
            course.MaxCapacity,
            course.EnrollmentCount,
            links);

        return Ok(detailDto);
    }
}