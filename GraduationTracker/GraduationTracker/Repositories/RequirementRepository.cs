using GraduationTracker.Models;
using System.Collections.Generic;

namespace GraduationTracker.Repositories
{
    /// <summary>
    /// Represents a repository for managing requirements.
    /// </summary>
    public class RequirementRepository : InMemoryRepository<Requirement>
    {
        /// <inheritdoc/>
        protected override IEnumerable<Requirement> Seed() =>
        [
            new Requirement{Id = 100, Name = "Math", MinimumMark=50, CourseIds = [1], CreditsAwarded=1 },
            new Requirement{Id = 102, Name = "Science", MinimumMark=50, CourseIds = [2], CreditsAwarded=1 },
            new Requirement{Id = 103, Name = "Literature", MinimumMark=50, CourseIds = [3], CreditsAwarded=1},
            new Requirement{Id = 104, Name = "Physical Education", MinimumMark=50, CourseIds = [4], CreditsAwarded=1 }
        ];
    }
}
