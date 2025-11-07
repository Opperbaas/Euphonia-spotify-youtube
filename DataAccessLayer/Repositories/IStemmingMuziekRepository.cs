using Euphonia.DataAccessLayer.Models;
using Euphonia.DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euphonia.DataAccessLayer.Repositories
{
    public interface IStemmingMuziekRepository : IRepository<StemmingMuziek>
    {
        Task<IEnumerable<StemmingMuziek>> GetMuziekByStemmingIdAsync(int stemmingId);
        Task<IEnumerable<StemmingMuziek>> GetStemmingenByMuziekIdAsync(int muziekId);
        Task<bool> IsAlreadyLinkedAsync(int stemmingId, int muziekId);
        Task DeleteByStemmingIdAsync(int stemmingId);
    }
}
