using EnrollmentLab.Models;
using EnrollmentLab.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnrollmentLab.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(
        IEnrollmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EnrollmentRecord>>>
    GetAll()
    {
        var records = await _service.GetAllAsync();

        return Ok(records);
    }
    // GET api/enrollments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<EnrollmentRecord>>
        GetById(string id)
    {
        var record =
            await _service.GetByIdAsync(id);

        if (record is null)
        {
            return NotFound();
        }

        return Ok(record);
    }

    // POST api/enrollments
[HttpPost]
public async Task<ActionResult<EnrollmentRecord>>
    Create(
        [FromBody]
        CreateEnrollmentRequest request)
{
    var record =
        await _service.EnrollAsync(
            request.StudentId,
            request.CourseCode);

    return CreatedAtAction(
        nameof(GetById),
        new { id = record.Id },
        record);
}
    // DELETE api/enrollments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        Delete(string id)
    {
        var removed =
            await _service.DeleteAsync(id);

        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}