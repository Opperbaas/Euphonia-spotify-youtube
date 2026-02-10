using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Repositories
{
    public class StemmingRepository : Repository<Stemming>, IStemmingRepository
    {
        public StemmingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Stemming>> GetStemmingenByUserIdAsync(int userId)
        {
            return await _context.Set<Stemming>()
                .Include(s => s.StemmingType)
                .Where(s => s.UserID == userId)
                .OrderByDescending(s => s.DatumTijd)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stemming>> GetStemmingenByTypeIdAsync(int typeId)
        {
            return await _context.Set<Stemming>()
                .Where(s => s.TypeID == typeId)
                .OrderByDescending(s => s.DatumTijd)
                .ToListAsync();
        }

        public async Task<Stemming?> GetStemmingMetDetailsAsync(int stemmingId)
        {
            return await _context.Set<Stemming>()
                .Include(s => s.StemmingType)
                .FirstOrDefaultAsync(s => s.StemmingID == stemmingId);
        }

        public async Task<Stemming?> GetLatestByUserAndTypeAsync(int userId, int typeId)
        {
            return await _context.Set<Stemming>()
                .Where(s => s.UserID == userId && s.TypeID == typeId)
                .OrderByDescending(s => s.DatumTijd)
                .FirstOrDefaultAsync();
        }
    }
}

