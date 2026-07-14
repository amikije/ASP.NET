namespace TmsApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using ASP.NET_session_3.Entities;
public interface ICourseService
{
    Task<Course?> GetByIdAsync(int id, CancellationToken ct);
    Task<Course> CreateAsync(Course course, CancellationToken ct);
}