using System.ComponentModel.DataAnnotations;

namespace TmsApi.Dtos.Enrollment;

public record EnrollStudentRequest
{
    [Required]
    public int StudentId { get; init; }
}