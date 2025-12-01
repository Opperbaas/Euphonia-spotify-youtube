using Euphonia.DataAccessLayer.Models;
using Euphonia.DataAccessLayer.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euphonia.DataAccessLayer.Repositories
{
    public interface IStemmingTypeRepository : IRepository<StemmingType>
    {
        Task<StemmingType?> GetByNaamAsync(string naam);
        Task<IEnumerable<StemmingType>> GetAllTypesAsync();
    }
}
