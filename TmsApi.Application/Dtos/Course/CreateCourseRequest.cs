using System.ComponentModel.DataAnnotations;

namespace TmsApi.Application.Dtos.Course;

public record CreateCourseRequest
{
    [Required]
    [MaxLength(10)]
    public required string Code { get; init; }

    [Required]
    [MaxLength(200)]
    public required string Title { get; init; }

    [Range(1, 500)]
    public int MaxCapacity { get; init; }
}