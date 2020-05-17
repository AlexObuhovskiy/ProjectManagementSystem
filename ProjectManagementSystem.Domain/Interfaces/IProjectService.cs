using System.Threading.Tasks;
using ProjectManagementSystem.Domain.Models.Project;

namespace ProjectManagementSystem.Domain.Interfaces
{
    public interface IProjectService
    {
        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns>Task&lt;ProjectResponseDto[]&gt;.</returns>
        Task<ProjectResponseDto[]> GetAllProjects();
    }
}