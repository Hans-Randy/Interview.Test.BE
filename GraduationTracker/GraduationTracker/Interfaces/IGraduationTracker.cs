using GraduationTracker.Models;

namespace GraduationTracker.Interfaces
{
    /// <summary>
    /// Defines the contract for the graduation tracking service.
    /// </summary>
    public interface IGraduationTracker
    {
        /// <summary>
        /// Checks if a student has graduated based on the diploma requirements and their courses.
        /// </summary>
        /// <param name="diploma">The diploma to check against.</param>
        /// <param name="student">The student to check.</param>
        /// <returns>A <see cref="GraduationResult"/> indicating graduation status and academic standing.</returns>
        GraduationResult HasGraduated(Diploma diploma, Student student);
    }
}