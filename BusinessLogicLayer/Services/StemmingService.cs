using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.DataAccessLayer.Models;
using Euphonia.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euphonia.BusinessLogicLayer.Services
{
    public class StemmingService : IStemmingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StemmingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StemmingDto>> GetAllStemmingenAsync()
        {
            var stemmingen = await _unitOfWork.StemmingRepository.GetAllAsync();
            return stemmingen.Select(MapToDto);
        }

        public async Task<IEnumerable<StemmingDto>> GetStemmingenByUserIdAsync(int userId)
        {
            var stemmingen = await _unitOfWork.StemmingRepository.GetStemmingenByUserIdAsync(userId);
            return stemmingen.Select(MapToDto);
        }

        public async Task<StemmingDto?> GetStemmingByIdAsync(int stemmingId)
        {
            var stemming = await _unitOfWork.StemmingRepository.GetStemmingMetDetailsAsync(stemmingId);
            return stemming != null ? MapToDto(stemming) : null;
        }

        public async Task<StemmingDto> CreateStemmingAsync(CreateStemmingDto createDto, int userId)
        {
            // Controleer of er al een bestaande stemming voor deze user + type is
            var existing = await _unitOfWork.StemmingRepository.GetLatestByUserAndTypeAsync(userId, createDto.TypeID);

            if (existing != null)
            {
                // Voeg nieuwe muziek toe aan bestaande stemming (vermijd duplicaten)
                if (createDto.MuziekIDs != null && createDto.MuziekIDs.Count > 0)
                {
                    foreach (var muziekId in createDto.MuziekIDs)
                    {
                        if (!await _unitOfWork.StemmingMuziekRepository.IsAlreadyLinkedAsync(existing.StemmingID, muziekId))
                        {
                            var link = new StemmingMuziek
                            {
                                StemmingID = existing.StemmingID,
                                MuziekID = muziekId
                            };
                            await _unitOfWork.StemmingMuziekRepository.AddAsync(link);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                // Optioneel: update beschrijving (append als er nieuwe tekst is)
                if (!string.IsNullOrWhiteSpace(createDto.Beschrijving))
                {
                    if (string.IsNullOrWhiteSpace(existing.Beschrijving))
                    {
                        existing.Beschrijving = createDto.Beschrijving;
                    }
                    else if (!existing.Beschrijving.Contains(createDto.Beschrijving))
                    {
                        existing.Beschrijving += "\n" + createDto.Beschrijving;
                    }
                    await _unitOfWork.StemmingRepository.UpdateAsync(existing);
                    await _unitOfWork.SaveChangesAsync();
                }

                var updated = await _unitOfWork.StemmingRepository.GetStemmingMetDetailsAsync(existing.StemmingID);
                return await MapToDtoWithMuziekAsync(updated!);
            }

            // Anders: maak een nieuwe stemming zoals voorheen
            var stemming = new Stemming
            {
                UserID = userId,
                TypeID = createDto.TypeID,
                Beschrijving = createDto.Beschrijving,
                DatumTijd = DateTime.Now
            };

            await _unitOfWork.StemmingRepository.AddAsync(stemming);
            await _unitOfWork.SaveChangesAsync();

            // Koppel muziek als er IDs zijn meegegeven
            if (createDto.MuziekIDs != null && createDto.MuziekIDs.Count > 0)
            {
                foreach (var muziekId in createDto.MuziekIDs)
                {
                    var link = new StemmingMuziek
                    {
                        StemmingID = stemming.StemmingID,
                        MuziekID = muziekId
                    };
                    await _unitOfWork.StemmingMuziekRepository.AddAsync(link);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            // Haal de volledige stemming op met details
            var savedStemming = await _unitOfWork.StemmingRepository.GetStemmingMetDetailsAsync(stemming.StemmingID);
            return await MapToDtoWithMuziekAsync(savedStemming!);
        }

        public async Task<bool> UpdateStemmingAsync(UpdateStemmingDto updateDto, int userId)
        {
            var stemming = await _unitOfWork.StemmingRepository.GetByIdAsync(updateDto.StemmingID);
            
            if (stemming == null || stemming.UserID != userId)
            {
                return false; // Stemming bestaat niet of hoort niet bij deze gebruiker
            }

            stemming.TypeID = updateDto.TypeID;
            stemming.Beschrijving = updateDto.Beschrijving;

            await _unitOfWork.StemmingRepository.UpdateAsync(stemming);

            // Update muziek koppelingen
            if (updateDto.MuziekIDs != null)
            {
                // Verwijder oude koppelingen
                await _unitOfWork.StemmingMuziekRepository.DeleteByStemmingIdAsync(updateDto.StemmingID);
                
                // Voeg nieuwe koppelingen toe
                foreach (var muziekId in updateDto.MuziekIDs)
                {
                    var link = new StemmingMuziek
                    {
                        StemmingID = updateDto.StemmingID,
                        MuziekID = muziekId
                    };
                    await _unitOfWork.StemmingMuziekRepository.AddAsync(link);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteStemmingAsync(int stemmingId, int userId)
        {
            var stemming = await _unitOfWork.StemmingRepository.GetByIdAsync(stemmingId);
            
            if (stemming == null || stemming.UserID != userId)
            {
                return false; // Stemming bestaat niet of hoort niet bij deze gebruiker
            }

            await _unitOfWork.StemmingRepository.DeleteAsync(stemming);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<StemmingTypeDto>> GetAllStemmingTypesAsync()
        {
            var types = await _unitOfWork.StemmingTypeRepository.GetAllTypesAsync();
            return types.Select(t => new StemmingTypeDto
            {
                TypeID = t.TypeID,
                Naam = t.Naam,
                Beschrijving = t.Beschrijving
            });
        }

        public async Task<bool> LinkMuziekToStemmingAsync(int stemmingId, int muziekId, int userId)
        {
            var stemming = await _unitOfWork.StemmingRepository.GetByIdAsync(stemmingId);
            if (stemming == null || stemming.UserID != userId)
            {
                return false;
            }

            // Check of de link al bestaat
            if (await _unitOfWork.StemmingMuziekRepository.IsAlreadyLinkedAsync(stemmingId, muziekId))
            {
                return false; // Al gekoppeld
            }

            var link = new StemmingMuziek
            {
                StemmingID = stemmingId,
                MuziekID = muziekId
            };

            await _unitOfWork.StemmingMuziekRepository.AddAsync(link);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnlinkMuziekFromStemmingAsync(int stemmingId, int muziekId, int userId)
        {
            var stemming = await _unitOfWork.StemmingRepository.GetByIdAsync(stemmingId);
            if (stemming == null || stemming.UserID != userId)
            {
                return false;
            }

            var links = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemmingId);
            var linkToRemove = links.FirstOrDefault(l => l.MuziekID == muziekId);

            if (linkToRemove == null)
            {
                return false;
            }

            await _unitOfWork.StemmingMuziekRepository.DeleteAsync(linkToRemove);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<MuziekDto>> GetMuziekByStemmingIdAsync(int stemmingId)
        {
            var links = await _unitOfWork.StemmingMuziekRepository.GetMuziekByStemmingIdAsync(stemmingId);
            return links.Where(l => l.Muziek != null).Select(l => new MuziekDto
            {
                MuziekID = l.Muziek!.MuziekID,
                Titel = l.Muziek.Titel,
                Artiest = l.Muziek.Artiest,
                Bron = l.Muziek.Bron
            });
        }

        private StemmingDto MapToDto(Stemming stemming)
        {
            return new StemmingDto
            {
                StemmingID = stemming.StemmingID,
                UserID = stemming.UserID,
                TypeID = stemming.TypeID,
                DatumTijd = stemming.DatumTijd,
                Beschrijving = stemming.Beschrijving,
                TypeNaam = stemming.StemmingType?.Naam
            };
        }

        private async Task<StemmingDto> MapToDtoWithMuziekAsync(Stemming stemming)
        {
            var dto = MapToDto(stemming);
            var muziek = await GetMuziekByStemmingIdAsync(stemming.StemmingID);
            dto.GekoppeldeMuziek = muziek.ToList();
            return dto;
        }
    }
}
