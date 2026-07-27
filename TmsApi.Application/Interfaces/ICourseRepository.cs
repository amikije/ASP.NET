using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IEnumerable<Course>> GetAllAsync(CancellationToken ct = default);
    Task<Course?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Course course, CancellationToken ct = default);
    Task UpdateAsync(Course course, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}