using Resonance.BusinessLogicLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resonance.BusinessLogicLayer.Services
{
    public interface IStemmingService
    {
        Task<IEnumerable<StemmingDto>> GetAllStemmingenAsync();
        Task<IEnumerable<StemmingDto>> GetStemmingenByUserIdAsync(int userId);
        Task<StemmingDto?> GetStemmingByIdAsync(int stemmingId);
        Task<StemmingDto> CreateStemmingAsync(CreateStemmingDto createDto, int userId);
        Task<bool> UpdateStemmingAsync(UpdateStemmingDto updateDto, int userId);
        Task<bool> DeleteStemmingAsync(int stemmingId, int userId);
        Task<IEnumerable<StemmingTypeDto>> GetAllStemmingTypesAsync();
        
        // Muziek koppeling
        Task<bool> LinkMuziekToStemmingAsync(int stemmingId, int muziekId, int userId);
        Task<bool> UnlinkMuziekFromStemmingAsync(int stemmingId, int muziekId, int userId);
        Task<IEnumerable<MuziekDto>> GetMuziekByStemmingIdAsync(int stemmingId);
    }
}

