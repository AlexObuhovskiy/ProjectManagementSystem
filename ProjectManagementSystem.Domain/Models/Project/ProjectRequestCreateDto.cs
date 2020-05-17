using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Domain.Models.Project
{
    /// <summary>
    /// Dto to create a project.
    /// </summary>
    public class ProjectRequestCreateDto
    {
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [Required]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; }
    }
}
