namespace TmsApi.Application.Dtos.Enrollment;

public record EnrollmentResponseDto
(
    int Id,
    int StudentId,
    int CourseId,
    DateTime EnrolledAt
);