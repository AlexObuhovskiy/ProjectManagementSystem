using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;

namespace ProjectManagementSystem.DataAccess.Extensions
{
    /// <summary>
    /// Class GenericRepositoryForProjectExtensions.
    /// </summary>
    public static class GenericRepositoryForProjectExtensions
    {
        /// <summary>
        /// Gets the task with all children.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Models.Project.</returns>
        public static async Task<Project> GetProjectWithAllChildren(
            this IGenericRepository<Project> repository,
            int id)
        {
            var entity = await repository.GetByIdAsync(id);
            GetChildren(entity, repository);
            return entity;
        }

        private static void GetChildren(Project parent, IGenericRepository<Project> repository)
        {
            repository.Context.Entry(parent).Collection(e => e.InverseParent).Query().Load();

            if (parent.InverseParent == null)
            {
                return;
            }

            foreach (Project child in parent.InverseParent)
            {
                GetChildren(child, repository);
            }
        }
    }
}