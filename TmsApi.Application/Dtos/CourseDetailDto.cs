namespace TmsApi.Application.DTOs;

public record CourseDetailDto(
    int Id,
    string Title,
    string Code,
    int MaxCapacity,
    int EnrollmentCount,
    List<EnrollmentDto> Enrollments);  // This is a record with positional parameters, so no warning

// If you're using a class instead of record, use this:
public class CourseDetailDtoClass
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public int MaxCapacity { get; init; }
    public int EnrollmentCount { get; init; }
    public List<EnrollmentDto> Enrollments { get; init; } = new(); // Initialize to avoid warning

    public CourseDetailDtoClass(
        int id,
        string title,
        string code,
        int maxCapacity,
        int enrollmentCount,
        List<EnrollmentDto> enrollments)
    {
        Id = id;
        Title = title;
        Code = code;
        MaxCapacity = maxCapacity;
        EnrollmentCount = enrollmentCount;
        Enrollments = enrollments ?? new List<EnrollmentDto>();
    }
}

public record EnrollmentDto(
    int Id,
    int StudentId,
    DateTime EnrolledAt);