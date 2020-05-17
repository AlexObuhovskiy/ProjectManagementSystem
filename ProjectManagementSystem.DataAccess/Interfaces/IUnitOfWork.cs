using System.Threading.Tasks;

namespace ProjectManagementSystem.DataAccess.Interfaces
{
    /// <summary>
    /// IUnitOfWork for using unit of work pattern
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="T">Repository type.</typeparam>
        /// <returns>IGenericRepository&lt;T&gt;.</returns>
        IGenericRepository<T> GetRepository<T>() where T : class, new();

        /// <summary>
        /// Save changes for DB context as an asynchronous operation.
        /// </summary>
        Task SaveChangesAsync();
    }
}