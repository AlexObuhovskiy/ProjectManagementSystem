using System.Collections.Generic;
using System.Linq;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;

namespace ProjectManagementSystem.DataAccess.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IGenericRepository{TEntity}"/>.
    /// </summary>
    public static class GenericRepositoryExtensions
    {
        /// <summary>
        /// Loads all children.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="list">The list.</param>
        public static void LoadAllChildren(
            this IGenericRepository<Project> repository,
            Project entity,
            List<Project> list = null)
        {
            repository.LoadAllChildren(entity, e => e.InverseParent, list);
        }

        /// <summary>
        /// Gets all task include sub projects.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="project">The project.</param>
        /// <param name="taskRepository">The task repository.</param>
        /// <returns>System.Threading.Tasks.Task&lt;Task[]&gt;.</returns>
        public static async System.Threading.Tasks.Task<Task[]> GetAllTaskIncludeSubProjects(
            this IGenericRepository<Project> repository,
            Project project,
            IGenericRepository<Task> taskRepository)
        {
            List<Project> list = new List<Project>();

            repository.LoadAllChildren(project, list);
            var projectIdList = list.Select(y => y.ProjectId).ToList();

            var allTasks = (await taskRepository.Get(x => projectIdList.Contains(x.ProjectId))).ToArray();

            return allTasks;
        }

        /// <summary>
        /// Loads all children for task.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="entity">The entity.</param>
        public static void LoadAllChildren(
            this IGenericRepository<Task> repository,
            Task entity)
        {
            repository.LoadAllChildren(entity, e => e.InverseParent);
        }
    }
}