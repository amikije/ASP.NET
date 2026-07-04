namespace ASP.NET_session_3.Entities;

public class Student
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public decimal GPA { get; set; }

    public bool IsActive { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}