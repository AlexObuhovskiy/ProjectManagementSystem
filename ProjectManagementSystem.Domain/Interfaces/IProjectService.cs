using System.Threading.Tasks;
using ProjectManagementSystem.Domain.Models.Project;

namespace ProjectManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// IProjectService for work with projects
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns>Task&lt;ProjectResponseDto[]&gt;.</returns>
        Task<ProjectResponseDto[]> GetAllProjects();

        /// <summary>
        /// Gets the project by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;ProjectResponseDto&gt;.</returns>
        Task<ProjectResponseDto> GetById(int id);

        /// <summary>
        /// Creates the specified project by request DTO.
        /// </summary>
        /// <param name="projectRequestCreateDto">The project request DTO.</param>
        /// <returns>Task&lt;ProjectResponseDto&gt;.</returns>
        Task<ProjectResponseDto> Create(ProjectRequestCreateDto projectRequestCreateDto);

        /// <summary>
        /// Updates the specified project by request create DTO.
        /// </summary>
        /// <param name="projectRequestUpdateDto">The project request update DTO.</param>
        /// <returns>Task&lt;ProjectResponseDto&gt;.</returns>
        Task<ProjectResponseDto> Update(ProjectRequestUpdateDto projectRequestUpdateDto);

        /// <summary>
        /// Deletes the specified project by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        Task Delete(int id);
    }
}