using System.Collections.Generic;

namespace Tms.Api.Dtos;

public record CourseDetailDto(
    int Id,
    string Code,
    string Title,
    int MaxCapacity,
    int EnrollmentCount,
    IReadOnlyList<LinkDto> Links);