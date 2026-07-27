using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TmsApi.Api.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/search")]
[ApiVersion("2.0")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("courses")]
    [EnableRateLimiting("search")]
    public async Task<IActionResult> SearchCourses(
        [FromQuery] string? term,
        CancellationToken ct)
    {
        // This is a stub - you can implement actual search logic
        var results = new List<object>();
        
        if (!string.IsNullOrEmpty(term))
        {
            // Simulate search - you'd use your repository/mediator here
            results.Add(new { id = 1, title = "Sample Course", code = "CSE-101" });
        }

        return Ok(new { results });
    }
}