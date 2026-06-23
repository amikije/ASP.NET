using System.ComponentModel.DataAnnotations;

namespace EnrollmentLab.Options;

public class AssessmentOptions
{
    [Range(1, 100)]
    public int PassMark { get; set; }

    [Range(1, 10)]
    public int MaxAttempts { get; set; }
}