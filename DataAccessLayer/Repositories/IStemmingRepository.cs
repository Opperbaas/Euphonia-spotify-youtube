using Euphonia.DataAccessLayer.Models;
using Euphonia.DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euphonia.DataAccessLayer.Repositories
{
    public interface IStemmingRepository : IRepository<Stemming>
    {
        Task<IEnumerable<Stemming>> GetStemmingenByUserIdAsync(int userId);
        Task<IEnumerable<Stemming>> GetStemmingenByTypeIdAsync(int typeId);
        Task<Stemming?> GetStemmingMetDetailsAsync(int stemmingId);
    }
}
