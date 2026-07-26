using TmsApi.Application.Dtos;
using TmsApi.Application.Dtos.Course;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Services;
using Microsoft.EntityFrameworkCore;
namespace TmsApi.Infrastructure.Services;


public class CourseService(
    TmsDbContext context,
    ILogger<CourseService> logger)
    :ICourseService
{
    public Task<bool> CodeExistsAsync(string code, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<CourseResponseDto?> GetByIdAsync(
       int id,
       CancellationToken ct)
    {
        return await context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count))
            .FirstOrDefaultAsync(ct);
    }
    // your other methods...

    public async Task<PagedResponse<CourseResponseDto>> GetCoursesAsync(
        PagedRequest request,
        CancellationToken ct)
    {
        IQueryable<Course> query = context.Courses
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.Title, $"%{request.Search}%") ||
                EF.Functions.ILike(c.Code, $"%{request.Search}%"));
        }

        var totalCount = await query.CountAsync(ct);

        IQueryable<Course> sortedQuery = request.OrderBy switch
        {
            "Code" => request.Descending
                ? query.OrderByDescending(c => c.Code)
                : query.OrderBy(c => c.Code),

            "MaxCapacity" => request.Descending
                ? query.OrderByDescending(c => c.MaxCapacity)
                : query.OrderBy(c => c.MaxCapacity),

            _ => request.Descending
                ? query.OrderByDescending(c => c.Title)
                : query.OrderBy(c => c.Title)
        };

        var items = await sortedQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseResponseDto(
                c.Id,
                c.Code,
                c.Title,
                c.MaxCapacity,
                c.Enrollments.Count))
            .ToListAsync(ct);

        return new PagedResponse<CourseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}