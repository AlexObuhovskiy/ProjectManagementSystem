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
        /// Gets all project identifier array.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="project">The project.</param>
        /// <returns>System.Int32[].</returns>
        public static int[] GetAllProjectIdArray(
            this IGenericRepository<Project> repository,
            Project project)
        {
            List<Project> list = new List<Project>();
            repository.LoadAllChildren(project, list);

            return list.Select(y => y.ProjectId).ToArray();
        }

        /// <summary>
        /// Gets all task of projects by project's identifiers.
        /// </summary>
        /// <param name="taskRepository">The task repository.</param>
        /// <param name="projectIdArray">The project identifier array.</param>
        /// <returns>System.Threading.Tasks.Task&lt;Task[]&gt;.</returns>
        public static async System.Threading.Tasks.Task<Task[]> GetAllTaskIncludeSubProjects(
            this IGenericRepository<Task> taskRepository,
            int[] projectIdArray)
        {
            var allTasks = (await taskRepository.Get(x => projectIdArray.Contains(x.ProjectId))).ToArray();

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