using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository voor Profiel data access
    /// </summary>
    public class ProfielRepository : Repository<Profiel>, IProfielRepository
    {
        public ProfielRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Profiel?> GetByUserIDAsync(int userID)
        {
            return await _context.Set<Profiel>()
                .FirstOrDefaultAsync(p => p.UserID == userID);
        }

        public async Task<Profiel?> GetActiveByUserIdAsync(int userId)
        {
            return await _context.Set<Profiel>()
                .FirstOrDefaultAsync(p => p.UserID == userId && p.IsActive);
        }

        public async Task<IEnumerable<Profiel>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Set<Profiel>()
                .Where(p => p.UserID == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Profiel>> GetByGenreAsync(string genre)
        {
            return await _context.Set<Profiel>()
                .Where(p => p.VoorkeurGenres != null && p.VoorkeurGenres.Contains(genre))
                .ToListAsync();
        }
    }
}

