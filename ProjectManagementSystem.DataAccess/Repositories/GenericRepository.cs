using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.DataAccess.Repositories
{
    /// <summary>
    /// Class GenericRepository.
    /// Implements the <see cref="IGenericRepository{TEntity}" />
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <seealso cref="IGenericRepository{TEntity}" />
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        /// <inhertidoc/>
        protected readonly ProjectManagementSystemContext Context;

        /// <inhertidoc/>
        public DbSet<TEntity> DbSet { get; }

        /// <inhertidoc/>
        public GenericRepository(ProjectManagementSystemContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        /// <inhertidoc/>
        public Task<TEntity[]> Get(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.ToArrayAsync();
        }

        /// <inhertidoc/>
        public Task<TEntity> GetByIdAsync(object id)
        {
            return DbSet.FindAsync(id).AsTask();
        }

        /// <inhertidoc/>
        public Task InsertAsync(TEntity entity)
        {
            return DbSet.AddAsync(entity).AsTask();
        }

        /// <inhertidoc/>
        public void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <inhertidoc/>
        public void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }

            DbSet.Remove(entityToDelete);
        }

        /// <inhertidoc/>
        public void Update(TEntity entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}