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
        public GraduationResult HasGraduated(Diploma diploma, Student student)
        {
            var credits = 0;
            
            var requirements = diploma.Requirements.Select(reqId => _requirementRepository.GetById(reqId));
            
            var studentCourses = student.Courses.ToList();

            foreach (var requirement in requirements)
            {
                var requirementCourse = studentCourses.FirstOrDefault(c => requirement.Courses.Contains(c.Id));
                if (requirementCourse != null && requirementCourse.Mark >= requirement.MinimumMark)
                {
                    credits += requirement.Credits;
                }
            }

            var average = student.Courses.Select(c => c.Mark).Average();

            var standing = Standing.None;

            if (average < 50)
                standing = Standing.Remedial;
            else if (average < 80)
                standing = Standing.Average;
            else if (average < 95)
                standing = Standing.MagnaCumLaude;
            else
                standing = Standing.SummaCumLaude;

            var hasEnoughCredits = credits >= diploma.Credits;
            var hasPassingStanding = standing != Standing.Remedial && standing != Standing.None;

            return new GraduationResult(hasEnoughCredits && hasPassingStanding, standing);
        }
    }
}
