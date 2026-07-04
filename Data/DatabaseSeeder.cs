using ASP.NET_session_3.Entities;


namespace TmsApi.Data;

public static class DatabaseSeeder
{
    public static void Seed(TmsDbContext context)
    {
        if (context.Students.Any())
            return;

        var students = new List<Student>
        {
            new Student { Name = "Osama", GPA = 3.8m, IsActive = true },
            new Student { Name = "Ali", GPA = 3.2m, IsActive = true },
            new Student { Name = "Ahmed", GPA = 2.5m, IsActive = false }
        };

        var courses = new List<Course>
        {
            new Course { Title = "C#", Credits = 3, IsActive = true },
            new Course { Title = "Database", Credits = 4, IsActive = true }
        };

        context.Students.AddRange(students);
        context.Courses.AddRange(courses);

        context.SaveChanges();

        var enrollments = new List<Enrollment>
        {
            new Enrollment
            {
                StudentId = students[0].Id,
                CourseId = courses[0].Id,
                EnrolledAt = DateTime.UtcNow
            },
            new Enrollment
            {
                StudentId = students[1].Id,
                CourseId = courses[1].Id,
                EnrolledAt = DateTime.UtcNow
            }
        };

        context.Enrollments.AddRange(enrollments);

        context.SaveChanges();
    }
}