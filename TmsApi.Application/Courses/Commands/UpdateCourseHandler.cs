using MediatR;
using TmsApi.Application.Interfaces;

namespace TmsApi.Application.Courses.Commands;

public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, bool>
{
    private readonly ICourseRepository _repo;
    private readonly ICachedCourseService _cachedService;

    public UpdateCourseHandler(ICourseRepository repo, ICachedCourseService cachedService)
    {
        _repo = repo;
        _cachedService = cachedService;
    }

    public async Task<bool> Handle(UpdateCourseCommand command, CancellationToken ct)
    {
        var course = await _repo.GetByIdAsync(command.Id, ct);
        if (course is null)
            return false;

        course.Title = command.Title;
        course.MaxCapacity = command.MaxCapacity;

        await _repo.UpdateAsync(course, ct);

        // Invalidate cache after write
        await _cachedService.InvalidateCourseCacheAsync(ct);

        return true;
    }
}