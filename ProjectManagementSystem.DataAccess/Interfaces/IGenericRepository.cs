using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ProjectManagementSystem.DataAccess.Models;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.DataAccess.Interfaces
{
    /// <summary>
    /// Interface IGenericRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        ProjectManagementSystemContext Context { get; }

        /// <summary>
        /// DB set for TEntity
        /// </summary>
        DbSet<TEntity> DbSet { get; }

        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include.</param>
        /// <returns>Task&lt;TEntity[]&gt;.</returns>
        Task<TEntity[]> Get(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);

        /// <summary>
        /// Gets entity by id asynchronous.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Inserts new entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task InsertAsync(TEntity entity);

        /// <summary>
        /// Deletes entity by id
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);

        /// <summary>
        /// Deletes entity
        /// </summary>
        /// <param name="entityToDelete"></param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Updates existing entity
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Loads all children.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="childrenExpression">The children expression.</param>
        /// <param name="list">The list.</param>
        void LoadAllChildren(
            TEntity entity,
            Expression<Func<TEntity, IEnumerable<TEntity>>> childrenExpression,
            List<TEntity> list = null);
    }
}
