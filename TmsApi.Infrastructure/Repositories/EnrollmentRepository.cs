using Microsoft.EntityFrameworkCore;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Data;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly TmsDbContext _context;

    public EnrollmentRepository(TmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.Course.Code == courseCode, ct);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        await _context.Enrollments.AddAsync(enrollment, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);
    }

    public async Task<Enrollment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var enrollment = await _context.Enrollments.FindAsync(id, ct);
        if (enrollment != null)
        {
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync(ct);
        }
    }
}