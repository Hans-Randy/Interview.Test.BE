using GraduationTracker.Models;
using System.Collections.Generic;

namespace GraduationTracker.Repositories
{
    /// <summary>
    /// Represents a repository for managing diplomas.
    /// </summary>
    public class DiplomaRepository : BaseRepository<Diploma>
    {
        /// <inheritdoc/>
        protected override IEnumerable<Diploma> Seed() =>
        [
            new Diploma
            {
                Id = 1,
                CreditsRequired = 4,
                RequirementIds = [100, 102, 103, 104]
            }
        ];
    }
}
