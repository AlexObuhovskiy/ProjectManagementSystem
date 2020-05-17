using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Domain.Models.Task
{
    /// <summary>
    /// Dto to create a task.
    /// </summary>
    public class TaskRequestCreateDto
    {
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>The parent identifier.</value>
        public int? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        [Required]
        public int? ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [Required]
        public string Description { get; set; }
    }
}