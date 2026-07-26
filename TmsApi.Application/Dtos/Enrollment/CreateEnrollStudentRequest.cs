using System.ComponentModel.DataAnnotations;

namespace TmsApi.Application.Dtos.Enrollment;

public record EnrollStudentRequest
{
    [Required]
    public int StudentId { get; init; }
}