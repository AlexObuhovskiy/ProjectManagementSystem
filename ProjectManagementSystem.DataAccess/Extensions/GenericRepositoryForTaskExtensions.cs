using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Interfaces;

namespace ProjectManagementSystem.DataAccess.Extensions
{
    /// <summary>
    /// Class GenericRepositoryForTaskExtensions.
    /// </summary>
    public static class GenericRepositoryForTaskExtensions
    {
        public static Models.Task GetTaskWithAllChildren(
            this IGenericRepository<Models.Task> repository,
            int id)
        {
            var entity = repository.DbSet.FirstOrDefault(e => e.TaskId == id);
            GetChildren(entity, repository);
            return entity;
        }

        private static void GetChildren(Models.Task parent, IGenericRepository<Models.Task> repository)
        {
            repository.Context.Entry(parent).Collection(e => e.InverseParent).Query().Load();

            if (parent.InverseParent != null)
            {
                foreach (Models.Task child in parent.InverseParent)
                {
                    GetChildren(child, repository);
                }
            }
        }
    }
}