using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Generic repository interface voor CRUD operaties
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        // CREATE
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // READ
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(System.Func<T, bool> predicate);

        // UPDATE
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        // DELETE
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);

        // EXTRA
        Task<int> CountAsync();
        Task<bool> ExistsAsync(int id);
    }
}
