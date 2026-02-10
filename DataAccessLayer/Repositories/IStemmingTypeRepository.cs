using Resonance.DataAccessLayer.Models;
using Resonance.DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Repositories
{
    public interface IStemmingTypeRepository : IRepository<StemmingType>
    {
        Task<StemmingType?> GetByNaamAsync(string naam);
        Task<IEnumerable<StemmingType>> GetAllTypesAsync();
    }
}

