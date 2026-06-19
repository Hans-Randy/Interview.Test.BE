using GraduationTracker.Models;

namespace GraduationTracker.Interfaces
{
    /// <summary>
    /// Defines the contract for the graduation tracking service.
    /// </summary>
    public interface IGraduationTracker
    {
        /// <summary>
        /// Evaluates a student against a diploma's requirements to determine graduation status and standing.
        /// </summary>
        /// <param name="diploma">The diploma to evaluate against.</param>
        /// <param name="student">The student to evaluate.</param>
        /// <returns>A <see cref="GraduationResult"/> indicating graduation status and academic standing.</returns>
        GraduationResult Evaluate(Diploma diploma, Student student);
    }
}