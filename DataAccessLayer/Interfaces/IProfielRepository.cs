using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Repository interface voor Profiel
    /// </summary>
    public interface IProfielRepository : IRepository<Profiel>
    {
        Task<Profiel?> GetByUserIDAsync(int userID);
        Task<IEnumerable<Profiel>> GetByGenreAsync(string genre);
    }
}
