using EnrollmentLab.Models;
using EnrollmentLab.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentLab.Controllers;

[ApiController]
[Route("api/enrollments")]
[Tags("Enrollments")]
[Produces("application/json")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service)
    {
        _service = service;
    }

    // GET api/enrollments
    [HttpGet]
    [EndpointSummary("Get all enrollments")]
    [EndpointDescription("Returns all enrollment records.")]
    [ProducesResponseType(typeof(IReadOnlyList<EnrollmentRecord>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EnrollmentRecord>>> GetAll()
    {
        var records = await _service.GetAllAsync();

        return Ok(records);
    }

    // GET api/enrollments/{id}
    [HttpGet("{id}")]
    [EndpointSummary("Get an enrollment by ID")]
    [EndpointDescription("Returns a single enrollment record by its identifier.")]
    [ProducesResponseType(typeof(EnrollmentRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentRecord>> GetById(string id)
    {
        var record = await _service.GetByIdAsync(id);

        if (record is null)
        {
            return NotFound();
        }

        return Ok(record);
    }

    // POST api/enrollments
    [HttpPost]
    [EndpointSummary("Create an enrollment")]
    [EndpointDescription("Enrolls a student into a course.")]
    [ProducesResponseType(typeof(EnrollmentRecord), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EnrollmentRecord>> Create(
        [FromBody] CreateEnrollmentRequest request)
    {
        var record = await _service.EnrollAsync(
            request.StudentId,
            request.CourseCode);

        return CreatedAtAction(
            nameof(GetById),
            new { id = record.Id },
            record);
    }

    // DELETE api/enrollments/{id}
    [HttpDelete("{id}")]
    [EndpointSummary("Delete an enrollment")]
    [EndpointDescription("Deletes an enrollment by its identifier.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var removed = await _service.DeleteAsync(id);

        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}