using System.Collections.Generic;
using System.Threading.Tasks;
using Euphonia.BusinessLogicLayer.DTOs;

namespace Euphonia.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Service interface voor business logic
    /// </summary>
    public interface ISpecificService
    {
        // CRUD operaties met DTOs
        Task<SpecificDto> GetByIdAsync(int id);
        Task<IEnumerable<SpecificDto>> GetAllAsync();
        Task<IEnumerable<SpecificDto>> GetActiveItemsAsync();
        Task<IEnumerable<SpecificDto>> SearchByNameAsync(string name);

        Task<SpecificDto> CreateAsync(CreateSpecificDto dto);
        Task<SpecificDto> UpdateAsync(UpdateSpecificDto dto);
        Task<bool> DeleteAsync(int id);

        // Business logic specifieke methods
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
        Task<int> GetActiveCountAsync();

        // Voeg hier je eigen business logic methods toe
    }
}
