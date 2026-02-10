using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Interface voor Notificatie repository
    /// </summary>
    public interface INotificatieRepository : IRepository<Notificatie>
    {
        /// <summary>
        /// Haal alle notificaties op voor een specifieke gebruiker
        /// </summary>
        Task<IEnumerable<Notificatie>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Haal ongelezen notificaties op voor een gebruiker
        /// </summary>
        Task<IEnumerable<Notificatie>> GetOngelezenByUserIdAsync(int userId);

        /// <summary>
        /// Tel ongelezen notificaties voor een gebruiker
        /// </summary>
        Task<int> CountOngelezenByUserIdAsync(int userId);

        /// <summary>
        /// Markeer notificatie als gelezen
        /// </summary>
        Task MarkeerAlsGelezenAsync(int notificatieId);

        /// <summary>
        /// Markeer alle notificaties van gebruiker als gelezen
        /// </summary>
        Task MarkeerAlleAlsGelezenAsync(int userId);

        /// <summary>
        /// Verwijder oude gelezen notificaties (ouder dan X dagen)
        /// </summary>
        Task VerwijderOudeGelezenNotificatiesAsync(int dagen = 30);
    }
}

