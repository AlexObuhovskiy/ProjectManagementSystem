using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.DataAccess.Repositories;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.DataAccess.UnitOfWork
{
    /// <summary>
    /// Class UnitOfWork.
    /// Implements the <see cref="IUnitOfWork" />
    /// </summary>
    /// <seealso cref="IUnitOfWork" />
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The EF core context
        /// </summary>
        private readonly ProjectManagementSystemContext _context;

        /// <summary>
        /// The loaded repositories
        /// </summary>
        private readonly Dictionary<Type, object> _loadedRepositories;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UnitOfWork(ProjectManagementSystemContext context)
        {
            _context = context;
            _loadedRepositories = new Dictionary<Type, object>();
        }

        /// <inhertidoc/>
        public IGenericRepository<T> GetRepository<T>() where T : class, new()
        {
            var repositoryType = typeof(T);
            if (_loadedRepositories.ContainsKey(repositoryType))
            {
                return (IGenericRepository<T>)_loadedRepositories[repositoryType];
            }

            var repository = new GenericRepository<T>(_context);
            _loadedRepositories.Add(repositoryType, repository);

            return repository;
        }

        /// <inhertidoc/>
        public async Task SaveChangesAsync()
        {
            try
            {
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }
    }
}