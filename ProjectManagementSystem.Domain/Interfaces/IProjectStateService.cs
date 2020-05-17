using System.Threading.Tasks;

namespace ProjectManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// IProjectStateService to check and change project's state
    /// </summary>
    public interface IProjectStateService
    {
        /// <summary>
        /// Checks the state of the project and project's parents to
        /// change state and start/finish dates if needed.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task.</returns>
        Task CheckProjectAndParentsToChangeState(int? id);
    }
}