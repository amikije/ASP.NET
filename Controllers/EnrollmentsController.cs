using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Dtos.Enrollment;
using TmsApi.Application.Services;

namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/enrollments")]
[Produces("application/json")]
[Tags("Enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EnrollmentResponseDto>>> GetAll(
        int courseId,
        CancellationToken ct)
    {
        var result = await _service.GetEnrollmentsAsync(courseId, ct);
        return Ok(result);
    }

    [HttpGet("{enrollmentId:int}")]
    public async Task<ActionResult<EnrollmentResponseDto>> GetById(
        int courseId,
        int enrollmentId,
        CancellationToken ct)
    {
        var result = await _service.GetEnrollmentAsync(
            courseId,
            enrollmentId,
            ct);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EnrollmentResponseDto>> Create(
        int courseId,
        [FromBody] EnrollStudentRequest request,
        CancellationToken ct)
    {
        var result = await _service.CreateEnrollmentAsync(
            courseId,
            request,
            ct);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                courseId,
                enrollmentId = result.Id
            },
            result);
    }

    [HttpDelete("{enrollmentId:int}")]
    public async Task<IActionResult> Delete(
        int courseId,
        int enrollmentId,
        CancellationToken ct)
    {
        var deleted = await _service.DeleteEnrollmentAsync(
            courseId,
            enrollmentId,
            ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}