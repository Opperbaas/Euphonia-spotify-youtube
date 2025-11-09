using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Euphonia.DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository voor Notificatie operaties
    /// </summary>
    public class NotificatieRepository : Repository<Notificatie>, INotificatieRepository
    {
        public NotificatieRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Haal alle notificaties op voor een specifieke gebruiker
        /// </summary>
        public async Task<IEnumerable<Notificatie>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.DatumTijd)
                .ToListAsync();
        }

        /// <summary>
        /// Haal ongelezen notificaties op voor een gebruiker
        /// </summary>
        public async Task<IEnumerable<Notificatie>> GetOngelezenByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(n => n.UserID == userId && !n.IsGelezen)
                .OrderByDescending(n => n.DatumTijd)
                .ToListAsync();
        }

        /// <summary>
        /// Tel ongelezen notificaties voor een gebruiker
        /// </summary>
        public async Task<int> CountOngelezenByUserIdAsync(int userId)
        {
            return await _dbSet
                .CountAsync(n => n.UserID == userId && !n.IsGelezen);
        }

        /// <summary>
        /// Markeer notificatie als gelezen
        /// </summary>
        public async Task MarkeerAlsGelezenAsync(int notificatieId)
        {
            var notificatie = await _dbSet.FindAsync(notificatieId);
            if (notificatie != null && !notificatie.IsGelezen)
            {
                notificatie.IsGelezen = true;
                _dbSet.Update(notificatie);
            }
        }

        /// <summary>
        /// Markeer alle notificaties van gebruiker als gelezen
        /// </summary>
        public async Task MarkeerAlleAlsGelezenAsync(int userId)
        {
            var ongelezenNotificaties = await _dbSet
                .Where(n => n.UserID == userId && !n.IsGelezen)
                .ToListAsync();

            foreach (var notificatie in ongelezenNotificaties)
            {
                notificatie.IsGelezen = true;
            }

            if (ongelezenNotificaties.Any())
            {
                _dbSet.UpdateRange(ongelezenNotificaties);
            }
        }

        /// <summary>
        /// Verwijder oude gelezen notificaties (ouder dan X dagen)
        /// </summary>
        public async Task VerwijderOudeGelezenNotificatiesAsync(int dagen = 30)
        {
            var grens = DateTime.Now.AddDays(-dagen);
            var oudeNotificaties = await _dbSet
                .Where(n => n.IsGelezen && n.DatumTijd < grens)
                .ToListAsync();

            if (oudeNotificaties.Any())
            {
                _dbSet.RemoveRange(oudeNotificaties);
            }
        }
    }
}
