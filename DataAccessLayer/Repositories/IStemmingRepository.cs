using Resonance.DataAccessLayer.Models;
using Resonance.DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Repositories
{
    public interface IStemmingRepository : IRepository<Stemming>
    {
        Task<IEnumerable<Stemming>> GetStemmingenByUserIdAsync(int userId);
        Task<IEnumerable<Stemming>> GetStemmingenByTypeIdAsync(int typeId);
        Task<Stemming?> GetStemmingMetDetailsAsync(int stemmingId);
        Task<Stemming?> GetLatestByUserAndTypeAsync(int userId, int typeId);
    }
}

