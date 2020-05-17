using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Interfaces;

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
        public static Models.Project GetProjectWithAllChildren(
            this IGenericRepository<Models.Project> repository,
            int id)
        {
            var entity = repository.DbSet.FirstOrDefault(e => e.ProjectId == id);
            GetChildren(entity, repository);
            return entity;
        }

        private static void GetChildren(Models.Project parent, IGenericRepository<Models.Project> repository)
        {
            repository.Context.Entry(parent).Collection(e => e.InverseParent).Query().Load();

            if (parent.InverseParent != null)
            {
                foreach (Models.Project child in parent.InverseParent)
                {
                    GetChildren(child, repository);
                }
            }
        }
    }
}