using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ASP.NET_session_3.Entities;

public class Student
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal GPA { get; set; }

    public bool IsActive { get; set; }

    public uint Version { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
}