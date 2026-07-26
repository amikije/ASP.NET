using Microsoft.EntityFrameworkCore;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Data;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly TmsDbContext _context;

    public CourseRepository(TmsDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Code == code, ct);
    }

    public async Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Course?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task AddAsync(Course course, CancellationToken ct = default)
    {
        await _context.Courses.AddAsync(course, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Course course, CancellationToken ct = default)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var course = await _context.Courses.FindAsync(id, ct);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync(ct);
        }
    }
}