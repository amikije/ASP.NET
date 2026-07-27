using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Dtos.Course;
using TmsApi.Application.DTOs;
using TmsApi.Application.Interfaces;
using TmsApi.Infrastructure.Caching;

namespace TmsApi.Infrastructure.Services;

public class CachedCourseService : ICachedCourseService
{
    private readonly HybridCache _cache;
    private readonly ICourseRepository _repo;
    private readonly ILogger<CachedCourseService> _logger;

    public CachedCourseService(
        HybridCache cache,
        ICourseRepository repo,
        ILogger<CachedCourseService> logger)
    {
        _cache = cache;
        _repo = repo;
        _logger = logger;
    }

    public async Task<CourseDetailDto> GetCourseAsync(string code, CancellationToken ct = default)
    {
        var key = CacheKeys.Course(code);
        var dbHit = false;

        var dto = await _cache.GetOrCreateAsync<CourseDetailDto>(
            key,
            async (token) =>
            {
                dbHit = true;
                _logger.LogInformation("Cache MISS for {Key} fetching from DB", key);

                var course = await _repo.GetByCodeAsync(code, token);
                if (course is null)
                    throw new InvalidOperationException($"Course {code} not found.");

                // Return CourseDetailDto - map from entity
                return new CourseDetailDto(
                    course.Id,
                    course.Title,
                    course.Code,
                    course.MaxCapacity,
                    course.Enrollments.Count,
                    course.Enrollments.Select(e => new EnrollmentDto(
                        e.Id,
                        e.StudentId,
                        e.EnrolledAt
                    )).ToList()
                );
            },
            cancellationToken: ct
        );

        if (!dbHit)
            _logger.LogInformation("Cache HIT for {Key}", key);

        return dto;
    }

    public async Task<List<CourseDetailDto>> GetAllCoursesAsync(CancellationToken ct = default)
    {
        var key = CacheKeys.CoursesAll;
        var dbHit = false;

        var list = await _cache.GetOrCreateAsync<List<CourseDetailDto>>(
            key,
            async (token) =>
            {
                dbHit = true;
                _logger.LogInformation("Cache MISS for {Key} fetching from DB", key);

                var courses = await _repo.GetAllAsync(token);

                // Map entities to DTOs
                return courses.Select(c => new CourseDetailDto(
                    c.Id,
                    c.Title,
                    c.Code,
                    c.MaxCapacity,
                    c.Enrollments.Count,
                    c.Enrollments.Select(e => new EnrollmentDto(
                        e.Id,
                        e.StudentId,
                        e.EnrolledAt
                    )).ToList()
                )).ToList();
            },
            cancellationToken: ct
        );

        if (!dbHit)
            _logger.LogInformation("Cache HIT for {Key}", key);

        return list;
    }

    public async Task InvalidateCourseCacheAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Invalidating cache tag {Tag}", CacheKeys.CoursesTag);
        await _cache.RemoveByTagAsync(CacheKeys.CoursesTag, ct);
    }
}