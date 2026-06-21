using GraduationTracker.Interfaces;
using GraduationTracker.Models;
using System.Linq;

namespace GraduationTracker.Services
{
    /// <summary>   
    ///     GraduationTracker is responsible for determining if a student has graduated based on their courses and the diploma requirements.
    /// </summary>
    public class GraduationTracker : IGraduationTracker
    {
        private readonly IRepository<Requirement> _requirementRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraduationTracker"/> class.
        /// </summary>
        /// <param name="requirementRepository">The requirement repository.</param>
        public GraduationTracker(IRepository<Requirement> requirementRepository)
        {
            _requirementRepository = requirementRepository;
        }

        /// <inheritdoc/>
        public GraduationResult Evaluate(Diploma diploma, Student student)
        {
            var studentCourses = student.Courses.ToList();

            bool IsMinimumMarkRequirementMet(Requirement requirement)
            {
                var course = studentCourses.FirstOrDefault(c => requirement.CourseIds.Contains(c.Id));
                return course != null && course.Mark >= requirement.MinimumMark;
            }

            var earnedCredits = diploma.RequirementIds
                .Select(_requirementRepository.GetById)
                .Where(IsMinimumMarkRequirementMet)
                .Sum(requirement => requirement.CreditsAwarded);

            var average = student.Courses.Select(c => c.Mark).Average();
            var standing = DetermineStanding(average);

            var hasEnoughCredits = earnedCredits >= diploma.CreditsRequired;
            var hasPassingStanding = standing != Standing.Remedial && standing != Standing.None;

            return new GraduationResult(hasEnoughCredits && hasPassingStanding, standing);
        }

        /// <summary>
        /// Maps an average mark to the corresponding academic <see cref="Standing"/>.
        /// </summary>
        /// <param name="average">The student's average mark across all courses.</param>
        /// <returns>The academic standing for that average.</returns>
        private static Standing DetermineStanding(double average)
        {
            const int RemedialCeiling = 50;
            const int AverageCeiling = 80;
            const int MagnaCumLaudeCeiling = 95;

            if (average < RemedialCeiling)
                return Standing.Remedial;
            if (average < AverageCeiling)
                return Standing.Average;
            if (average < MagnaCumLaudeCeiling)
                return Standing.MagnaCumLaude;

            return Standing.SummaCumLaude;
        }
    }
}
