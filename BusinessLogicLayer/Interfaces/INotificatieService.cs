using Resonance.BusinessLogicLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Interface voor Notificatie service
    /// </summary>
    public interface INotificatieService
    {
        /// <summary>
        /// Haal alle notificaties op voor een gebruiker
        /// </summary>
        Task<List<NotificatieDto>> GetNotificatiesByUserIdAsync(int userId);

        /// <summary>
        /// Haal ongelezen notificaties op
        /// </summary>
        Task<List<NotificatieDto>> GetOngelezenNotificatiesAsync(int userId);

        /// <summary>
        /// Haal notificatie overzicht op (voor badge in navbar)
        /// </summary>
        Task<NotificatieOverzichtDto> GetNotificatieOverzichtAsync(int userId);

        /// <summary>
        /// Maak nieuwe notificatie aan
        /// </summary>
        Task<NotificatieDto> CreateNotificatieAsync(CreateNotificatieDto createDto);

        /// <summary>
        /// Markeer notificatie als gelezen
        /// </summary>
        Task MarkeerAlsGelezenAsync(int notificatieId);

        /// <summary>
        /// Markeer alle notificaties als gelezen
        /// </summary>
        Task MarkeerAlleAlsGelezenAsync(int userId);

        /// <summary>
        /// Verwijder notificatie
        /// </summary>
        Task DeleteNotificatieAsync(int notificatieId);

        /// <summary>
        /// Verwijder oude gelezen notificaties
        /// </summary>
        Task VerwijderOudeNotificatiesAsync(int dagen = 30);

        /// <summary>
        /// Stuur welkomstnotificatie naar nieuwe gebruiker
        /// </summary>
        Task StuurWelkomstNotificatieAsync(int userId);

        /// <summary>
        /// Stuur notificatie wanneer stemming is toegevoegd
        /// </summary>
        Task StuurStemmingToegevoegdNotificatieAsync(int userId, string stemmingType);

        /// <summary>
        /// Stuur notificatie wanneer muziek is gekoppeld
        /// </summary>
        Task StuurMuziekGekoppeldNotificatieAsync(int userId, string muziekTitel);
    }
}

