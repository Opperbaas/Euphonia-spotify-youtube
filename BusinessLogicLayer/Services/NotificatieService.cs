using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resonance.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service voor notificatiebeheer
    /// </summary>
    public class NotificatieService : INotificatieService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificatieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Haal alle notificaties op voor een gebruiker
        /// </summary>
        public async Task<List<NotificatieDto>> GetNotificatiesByUserIdAsync(int userId)
        {
            var notificaties = await _unitOfWork.NotificatieRepository.GetByUserIdAsync(userId);
            return notificaties.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Haal ongelezen notificaties op
        /// </summary>
        public async Task<List<NotificatieDto>> GetOngelezenNotificatiesAsync(int userId)
        {
            var notificaties = await _unitOfWork.NotificatieRepository.GetOngelezenByUserIdAsync(userId);
            return notificaties.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Haal notificatie overzicht op (voor badge in navbar)
        /// </summary>
        public async Task<NotificatieOverzichtDto> GetNotificatieOverzichtAsync(int userId)
        {
            var alleNotificaties = await _unitOfWork.NotificatieRepository.GetByUserIdAsync(userId);
            var ongelezenCount = await _unitOfWork.NotificatieRepository.CountOngelezenByUserIdAsync(userId);

            return new NotificatieOverzichtDto
            {
                TotaalNotificaties = alleNotificaties.Count(),
                OngelezenNotificaties = ongelezenCount,
                RecenteNotificaties = alleNotificaties
                    .Take(5)
                    .Select(MapToDto)
                    .ToList()
            };
        }

        /// <summary>
        /// Maak nieuwe notificatie aan
        /// </summary>
        public async Task<NotificatieDto> CreateNotificatieAsync(CreateNotificatieDto createDto)
        {
            var notificatie = new Notificatie
            {
                UserID = createDto.UserID,
                Tekst = createDto.Tekst,
                Type = createDto.Type,
                Link = createDto.Link,
                Icoon = createDto.Icoon ?? GetDefaultIconForType(createDto.Type),
                DatumTijd = DateTime.Now,
                IsGelezen = false
            };

            await _unitOfWork.NotificatieRepository.AddAsync(notificatie);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(notificatie);
        }

        /// <summary>
        /// Markeer notificatie als gelezen
        /// </summary>
        public async Task MarkeerAlsGelezenAsync(int notificatieId)
        {
            await _unitOfWork.NotificatieRepository.MarkeerAlsGelezenAsync(notificatieId);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Markeer alle notificaties als gelezen
        /// </summary>
        public async Task MarkeerAlleAlsGelezenAsync(int userId)
        {
            await _unitOfWork.NotificatieRepository.MarkeerAlleAlsGelezenAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Verwijder notificatie
        /// </summary>
        public async Task DeleteNotificatieAsync(int notificatieId)
        {
            var notificatie = await _unitOfWork.NotificatieRepository.GetByIdAsync(notificatieId);
            if (notificatie != null)
            {
                await _unitOfWork.NotificatieRepository.DeleteAsync(notificatie);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verwijder oude gelezen notificaties
        /// </summary>
        public async Task VerwijderOudeNotificatiesAsync(int dagen = 30)
        {
            await _unitOfWork.NotificatieRepository.VerwijderOudeGelezenNotificatiesAsync(dagen);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Stuur welkomstnotificatie naar nieuwe gebruiker
        /// </summary>
        public async Task StuurWelkomstNotificatieAsync(int userId)
        {
            await CreateNotificatieAsync(new CreateNotificatieDto
            {
                UserID = userId,
                Tekst = "Welkom bij Resonance! Begin met het tracken van je stemmingen en ontdek patronen in je muziek.",
                Type = "Info",
                Icoon = "bi-star",
                Link = "/Dashboard"
            });
        }

        /// <summary>
        /// Stuur notificatie wanneer stemming is toegevoegd
        /// </summary>
        public async Task StuurStemmingToegevoegdNotificatieAsync(int userId, string stemmingType)
        {
            await CreateNotificatieAsync(new CreateNotificatieDto
            {
                UserID = userId,
                Tekst = $"Nieuwe stemming '{stemmingType}' toegevoegd! Koppel er muziek aan om je ervaring compleet te maken.",
                Type = "Success",
                Icoon = "bi-emoji-smile",
                Link = "/Stemming"
            });
        }

        /// <summary>
        /// Stuur notificatie wanneer muziek is gekoppeld
        /// </summary>
        public async Task StuurMuziekGekoppeldNotificatieAsync(int userId, string muziekTitel)
        {
            await CreateNotificatieAsync(new CreateNotificatieDto
            {
                UserID = userId,
                Tekst = $"Muziek '{muziekTitel}' succesvol gekoppeld aan je stemming!",
                Type = "Success",
                Icoon = "bi-music-note-beamed",
                Link = "/MuziekView"
            });
        }

        // Helper methods

        private NotificatieDto MapToDto(Notificatie notificatie)
        {
            return new NotificatieDto
            {
                NotificatieID = notificatie.NotificatieID,
                UserID = notificatie.UserID,
                Tekst = notificatie.Tekst,
                DatumTijd = notificatie.DatumTijd,
                Type = notificatie.Type,
                IsGelezen = notificatie.IsGelezen,
                Link = notificatie.Link,
                Icoon = notificatie.Icoon
            };
        }

        private string GetDefaultIconForType(string type)
        {
            return type?.ToLower() switch
            {
                "success" => "bi-check-circle",
                "warning" => "bi-exclamation-triangle",
                "error" => "bi-x-circle",
                _ => "bi-info-circle"
            };
        }
    }
}

