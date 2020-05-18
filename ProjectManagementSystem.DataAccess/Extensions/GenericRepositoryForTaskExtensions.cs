using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;

namespace ProjectManagementSystem.DataAccess.Extensions
{
    /// <summary>
    /// Class GenericRepositoryForTaskExtensions.
    /// </summary>
    public static class GenericRepositoryForTaskExtensions
    {
        /// <summary>
        /// Gets the task with all children.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Models.Task.</returns>
        public static Task GetTaskWithAllChildren(
            this IGenericRepository<Task> repository,
            int id)
        {
            var entity = repository.DbSet.FirstOrDefault(e => e.TaskId == id);
            GetChildren(entity, repository);
            return entity;
        }

        private static void GetChildren(Task parent, IGenericRepository<Task> repository)
        {
            repository.Context.Entry(parent).Collection(e => e.InverseParent).Query().Load();

            if (parent.InverseParent == null)
            {
                return;
            }

            foreach (Task child in parent.InverseParent)
            {
                GetChildren(child, repository);
            }
        }
    }
}