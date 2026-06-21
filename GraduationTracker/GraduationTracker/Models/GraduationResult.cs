namespace GraduationTracker.Models
{
    /// <summary>
    /// Represents the outcome of a graduation eligibility check.
    /// </summary>
    public record GraduationResult(bool HasGraduated, Standing Standing);
}
