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
        /// Gets or sets the total credits a student must earn to be awarded the diploma.
        /// </summary>
        public int CreditsRequired { get; set; }
        
        /// <summary>
        /// Gets or sets the identifiers of the requirements that make up the diploma.
        /// </summary>
        public int[] RequirementIds { get; set; }
    }
}
