using System.Collections.Generic;
using System.Threading.Tasks;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Specifieke repository interface - voorbeeld voor een entiteit
    /// Breid IRepository uit met custom methods
    /// </summary>
    public interface ISpecificRepository : IRepository<SpecificEntity>
    {
        // Custom methods specifiek voor deze entiteit
        Task<IEnumerable<SpecificEntity>> GetActiveItemsAsync();
        Task<IEnumerable<SpecificEntity>> GetByNameAsync(string name);
        Task<SpecificEntity> GetWithRelatedDataAsync(int id);
        
        // Voeg hier je eigen custom queries toe
    }
}
