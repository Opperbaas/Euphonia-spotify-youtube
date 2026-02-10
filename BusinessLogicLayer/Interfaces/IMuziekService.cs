using System.Collections.Generic;
using System.Threading.Tasks;
using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Service interface voor Muziek business logic
    /// </summary>
    public interface IMuziekService
    {
        // CRUD operaties
        Task<MuziekDto?> GetByIdAsync(int id);
        Task<MuziekDto?> GetWithAnalysesAsync(int id);
        Task<IEnumerable<MuziekDto>> GetAllAsync();
        Task<MuziekDto> CreateAsync(CreateMuziekDto dto);
        Task<MuziekDto?> UpdateAsync(UpdateMuziekDto dto);
        Task<bool> DeleteAsync(int id);

        // Zoek functionaliteit
        Task<IEnumerable<MuziekDto>> SearchAsync(string searchTerm);
        Task<IEnumerable<MuziekDto>> GetByArtiestAsync(string artiest);
        Task<IEnumerable<MuziekDto>> GetByTitelAsync(string titel);
    }
}

