using System.Threading.Tasks;
using ProjectManagementSystem.Domain.Models.Task;

namespace ProjectManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// ITaskService for work with tasks
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Gets all tasks.
        /// </summary>
        /// <returns>Task&lt;TaskResponseDto[]&gt;.</returns>
        Task<TaskResponseDto[]> GetAllTasks();

        /// <summary>
        /// Gets the task by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;TaskResponseDto&gt;.</returns>
        Task<TaskResponseDto> GetById(int id);

        /// <summary>
        /// Creates the specified task by request create dto.
        /// </summary>
        /// <param name="taskRequestCreateDto">The task request create dto.</param>
        /// <returns>Task&lt;TaskResponseDto&gt;.</returns>
        Task<TaskResponseDto> Create(TaskRequestCreateDto taskRequestCreateDto);
    }
}
