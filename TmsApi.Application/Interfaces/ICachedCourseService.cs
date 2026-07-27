using TmsApi.Application.Dtos.Course;
using TmsApi.Application.DTOs;


namespace TmsApi.Application.Interfaces;

public interface ICachedCourseService
{
    Task<CourseDetailDto> GetCourseAsync(string code, CancellationToken ct = default);
    Task<List<CourseDetailDto>> GetAllCoursesAsync(CancellationToken ct = default);
    Task InvalidateCourseCacheAsync(CancellationToken ct = default);
}