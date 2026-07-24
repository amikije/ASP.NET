using EnrollmentLab.Models;
using TmsApi.Dtos.Enrollment;

namespace TmsApi.Services;

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
        CreateEnrollmentRequest request,
        CancellationToken ct);

    Task<bool> DeleteEnrollmentAsync(
        int courseId,
        int enrollmentId,
        CancellationToken ct);
}