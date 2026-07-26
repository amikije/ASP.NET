using System.Collections.Generic;

namespace TmsApi.Application.Dtos.Course;

public record CourseDetailDto(
    int Id,
    string Code,
    string Title,
    int MaxCapacity,
    int EnrollmentCount,
    IReadOnlyList<LinkDto> Links);