using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Repository interface voor Profiel
    /// </summary>
    public interface IProfielRepository : IRepository<Profiel>
    {
        Task<Profiel?> GetByUserIDAsync(int userID);
        Task<Profiel?> GetActiveByUserIdAsync(int userId);
        Task<IEnumerable<Profiel>> GetAllByUserIdAsync(int userId);
        Task<IEnumerable<Profiel>> GetByGenreAsync(string genre);
    }
}

