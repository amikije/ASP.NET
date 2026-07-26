namespace TmsApi.Application.Dtos;

public class CreateEnrollmentRequest
{
    public string StudentId { get; set; } = "";

    public string CourseCode { get; set; } = "";
}