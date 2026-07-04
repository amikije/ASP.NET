namespace ASP.NET_session_3.Entities;

public class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public int Credits { get; set; }

    public bool IsActive { get; set; }
}