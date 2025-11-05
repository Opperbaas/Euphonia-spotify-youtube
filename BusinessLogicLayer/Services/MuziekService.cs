using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service voor Muziek business logic
    /// </summary>
    public class MuziekService : IMuziekService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MuziekService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MuziekDto?> GetByIdAsync(int id)
        {
            var muziek = await _unitOfWork.MuziekRepository.GetByIdAsync(id);
            return muziek == null ? null : MapToDto(muziek);
        }

        public async Task<MuziekDto?> GetWithAnalysesAsync(int id)
        {
            var muziek = await _unitOfWork.MuziekRepository.GetWithAnalysesAsync(id);
            return muziek == null ? null : MapToDtoWithAnalyses(muziek);
        }

        public async Task<IEnumerable<MuziekDto>> GetAllAsync()
        {
            var muziekList = await _unitOfWork.MuziekRepository.GetAllAsync();
            return muziekList.Select(MapToDto);
        }

        public async Task<MuziekDto> CreateAsync(CreateMuziekDto dto)
        {
            // Validatie
            ValidateCreateDto(dto);

            var muziek = new Muziek
            {
                Titel = dto.Titel,
                Artiest = dto.Artiest,
                Bron = dto.Bron
            };

            await _unitOfWork.MuziekRepository.AddAsync(muziek);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(muziek);
        }

        public async Task<MuziekDto?> UpdateAsync(UpdateMuziekDto dto)
        {
            ValidateUpdateDto(dto);

            var muziek = await _unitOfWork.MuziekRepository.GetByIdAsync(dto.MuziekID);
            if (muziek == null)
            {
                return null;
            }

            muziek.Titel = dto.Titel;
            muziek.Artiest = dto.Artiest;
            muziek.Bron = dto.Bron;

            await _unitOfWork.MuziekRepository.UpdateAsync(muziek);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(muziek);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var muziek = await _unitOfWork.MuziekRepository.GetByIdAsync(id);
            if (muziek == null)
            {
                return false;
            }

            await _unitOfWork.MuziekRepository.DeleteAsync(muziek);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<MuziekDto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var results = await _unitOfWork.MuziekRepository.SearchAsync(searchTerm);
            return results.Select(MapToDto);
        }

        public async Task<IEnumerable<MuziekDto>> GetByArtiestAsync(string artiest)
        {
            var results = await _unitOfWork.MuziekRepository.GetByArtiestAsync(artiest);
            return results.Select(MapToDto);
        }

        public async Task<IEnumerable<MuziekDto>> GetByTitelAsync(string titel)
        {
            var results = await _unitOfWork.MuziekRepository.GetByTitelAsync(titel);
            return results.Select(MapToDto);
        }

        // Private helper methods
        private MuziekDto MapToDto(Muziek muziek)
        {
            return new MuziekDto
            {
                MuziekID = muziek.MuziekID,
                Titel = muziek.Titel,
                Artiest = muziek.Artiest,
                Bron = muziek.Bron
            };
        }

        private MuziekDto MapToDtoWithAnalyses(Muziek muziek)
        {
            return new MuziekDto
            {
                MuziekID = muziek.MuziekID,
                Titel = muziek.Titel,
                Artiest = muziek.Artiest,
                Bron = muziek.Bron,
                Analyses = muziek.Analyses?.Select(a => new MuziekAnalyseDto
                {
                    AnalyseID = a.AnalyseID,
                    StemmingType = a.StemmingType,
                    EnergieLevel = a.EnergieLevel,
                    Valence = a.Valence,
                    Tempo = a.Tempo
                }).ToList()
            };
        }

        private void ValidateCreateDto(CreateMuziekDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Titel))
                throw new ArgumentException("Titel is verplicht", nameof(dto.Titel));

            if (string.IsNullOrWhiteSpace(dto.Artiest))
                throw new ArgumentException("Artiest is verplicht", nameof(dto.Artiest));
        }

        private void ValidateUpdateDto(UpdateMuziekDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.MuziekID <= 0)
                throw new ArgumentException("Ongeldig MuziekID", nameof(dto.MuziekID));

            if (string.IsNullOrWhiteSpace(dto.Titel))
                throw new ArgumentException("Titel is verplicht", nameof(dto.Titel));

            if (string.IsNullOrWhiteSpace(dto.Artiest))
                throw new ArgumentException("Artiest is verplicht", nameof(dto.Artiest));
        }
    }
}
