using System.ComponentModel.DataAnnotations;
using ProjectManagementSystem.Domain.Enums;

namespace ProjectManagementSystem.Domain.Models.Task
{
    /// <summary>
    /// DTO to update a task.
    /// </summary>
    public class TaskRequestUpdateDto
    {
        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>The task identifier.</value>
        [Required]
        public int? TaskId { get; set; }

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

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        [Required, EnumDataType(typeof(State), ErrorMessage = "State type value doesn't exist within enum")]
        public State? State { get; set; }
    }
}