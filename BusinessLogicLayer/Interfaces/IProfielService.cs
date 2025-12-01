using Euphonia.BusinessLogicLayer.DTOs;

namespace Euphonia.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Service interface voor Profiel business logic
    /// </summary>
    public interface IProfielService
    {
        Task<IEnumerable<ProfielDto>> GetAllAsync();
        Task<ProfielDto?> GetByIdAsync(int id);
        Task<ProfielDto?> GetByUserIDAsync(int userID);
        Task<IEnumerable<ProfielDto>> GetByGenreAsync(string genre);
        Task<ProfielDto> CreateAsync(CreateProfielDto dto);
        Task<ProfielDto?> UpdateAsync(UpdateProfielDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
