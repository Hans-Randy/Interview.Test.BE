using GraduationTracker.Interfaces;

namespace GraduationTracker.Models
{
    /// <summary>
    /// Represents a diploma with a set of requirements.
    /// </summary>
    public class Diploma : IEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the diploma.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the number of credits required for the diploma.
        /// </summary>
        public int NumberOfCreditsRequired { get; set; }
        
        /// <summary>
        /// Gets or sets the identifiers of the requirements that make up the diploma.
        /// </summary>
        public int[] RequirementIds { get; set; }
    }
}
