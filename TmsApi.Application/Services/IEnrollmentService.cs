using TmsApi.Application.Dtos.Enrollment;

namespace TmsApi.Application.Services;

public interface IEnrollmentService
{
    Task<IReadOnlyList<EnrollmentResponseDto>> GetEnrollmentsAsync(
        int courseId,
        CancellationToken ct);

    Task<EnrollmentResponseDto?> GetEnrollmentAsync(
        int courseId,
        int enrollmentId,
        CancellationToken ct);

    Task<EnrollmentResponseDto> CreateEnrollmentAsync(
        int courseId,
        EnrollStudentRequest request,
        CancellationToken ct);

    Task<bool> DeleteEnrollmentAsync(
        int courseId,
        int enrollmentId,
        CancellationToken ct);
}