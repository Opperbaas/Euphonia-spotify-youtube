using System.Collections.Generic;
using System.Threading.Tasks;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Repository interface voor Muziek entiteit
    /// </summary>
    public interface IMuziekRepository : IRepository<Muziek>
    {
        // Custom methods specifiek voor Muziek
        Task<IEnumerable<Muziek>> GetByArtiestAsync(string artiest);
        Task<IEnumerable<Muziek>> GetByTitelAsync(string titel);
        Task<Muziek?> GetWithAnalysesAsync(int id);
        Task<IEnumerable<Muziek>> SearchAsync(string searchTerm);
    }
}
