using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(TmsDbContext context)
    {
      
        if (context.Students.Any())
            return;

        // Seed Students
        var students = new List<Student>
        {
            new Student
            {
                Name = "Osama",
                GPA = 3.8m,
                IsActive = true
            },
            new Student
            {
                Name = "Ali",
                GPA = 3.2m,
                IsActive = true
            },
            new Student
            {
                Name = "Ahmed",
                GPA = 2.5m,
                IsActive = false
            }
        };

        // Seed Courses
        var courses = new List<Course>
        {
            new Course
            {
                Code = "CSE-101",
                Title = "C# Programming",
                MaxCapacity = 30
            },
            new Course
            {
                Code = "CSE-102",
                Title = "Database Systems",
                MaxCapacity = 25
            }
        };

        context.Students.AddRange(students);
        context.Courses.AddRange(courses);

        context.SaveChanges();

        // Seed Enrollments
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