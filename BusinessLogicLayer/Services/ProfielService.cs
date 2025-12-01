using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service voor Profiel business logic
    /// </summary>
    public class ProfielService : IProfielService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfielService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProfielDto>> GetAllAsync()
        {
            var profielen = await _unitOfWork.ProfielRepository.GetAllAsync();
            return profielen.Select(MapToDto);
        }

        public async Task<ProfielDto?> GetByIdAsync(int id)
        {
            var profiel = await _unitOfWork.ProfielRepository.GetByIdAsync(id);
            return profiel == null ? null : MapToDto(profiel);
        }

        public async Task<ProfielDto?> GetByUserIDAsync(int userID)
        {
            var profiel = await _unitOfWork.ProfielRepository.GetByUserIDAsync(userID);
            return profiel == null ? null : MapToDto(profiel);
        }

        public async Task<ProfielDto?> GetActiveProfielAsync(int userID)
        {
            var activeProfiel = await _unitOfWork.ProfielRepository.GetActiveByUserIdAsync(userID);
            return activeProfiel == null ? null : MapToDto(activeProfiel);
        }

        public async Task<bool> SetActiveProfielAsync(int userID, int profielID)
        {
            // Get all user's profiles
            var allProfiles = await _unitOfWork.ProfielRepository.GetAllByUserIdAsync(userID);
            var profilesList = allProfiles.ToList();

            // Find target profile
            var targetProfile = profilesList.FirstOrDefault(p => p.ProfielID == profielID);
            if (targetProfile == null) return false;

            // Deactivate all profiles
            foreach (var profile in profilesList)
            {
                profile.IsActive = false;
            }

            // Activate target profile
            targetProfile.IsActive = true;

            // Save changes
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProfielDto>> GetByGenreAsync(string genre)
        {
            var profielen = await _unitOfWork.ProfielRepository.GetByGenreAsync(genre);
            return profielen.Select(MapToDto);
        }

        public async Task<ProfielDto> CreateAsync(CreateProfielDto dto)
        {
            ValidateCreateDto(dto);

            var profiel = new Profiel
            {
                UserID = dto.UserID,
                VoorkeurGenres = dto.VoorkeurGenres,
                Stemmingstags = dto.Stemmingstags
            };

            await _unitOfWork.ProfielRepository.AddAsync(profiel);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(profiel);
        }

        public async Task<ProfielDto?> UpdateAsync(UpdateProfielDto dto)
        {
            ValidateUpdateDto(dto);

            var profiel = await _unitOfWork.ProfielRepository.GetByIdAsync(dto.ProfielID);
            if (profiel == null) return null;

            profiel.UserID = dto.UserID;
            profiel.VoorkeurGenres = dto.VoorkeurGenres;
            profiel.Stemmingstags = dto.Stemmingstags;

            await _unitOfWork.ProfielRepository.UpdateAsync(profiel);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(profiel);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var profiel = await _unitOfWork.ProfielRepository.GetByIdAsync(id);
            if (profiel == null) return false;

            await _unitOfWork.ProfielRepository.DeleteAsync(profiel);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Mapping methods
        private static ProfielDto MapToDto(Profiel profiel)
        {
            return new ProfielDto
            {
                ProfielID = profiel.ProfielID,
                UserID = profiel.UserID,
                VoorkeurGenres = profiel.VoorkeurGenres,
                Stemmingstags = profiel.Stemmingstags,
                IsActive = profiel.IsActive
            };
        }

        // Validation methods
        private static void ValidateCreateDto(CreateProfielDto dto)
        {
            // Add validation logic if needed
        }

        private static void ValidateUpdateDto(UpdateProfielDto dto)
        {
            if (dto.ProfielID <= 0)
                throw new ArgumentException("ProfielID moet groter zijn dan 0", nameof(dto.ProfielID));
        }
    }
}
