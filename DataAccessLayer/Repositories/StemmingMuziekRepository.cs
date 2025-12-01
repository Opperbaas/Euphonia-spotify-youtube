using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euphonia.DataAccessLayer.Repositories
{
    public class StemmingMuziekRepository : Repository<StemmingMuziek>, IStemmingMuziekRepository
    {
        public StemmingMuziekRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StemmingMuziek>> GetMuziekByStemmingIdAsync(int stemmingId)
        {
            return await _context.Set<StemmingMuziek>()
                .Include(sm => sm.Muziek)
                .Where(sm => sm.StemmingID == stemmingId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StemmingMuziek>> GetStemmingenByMuziekIdAsync(int muziekId)
        {
            return await _context.Set<StemmingMuziek>()
                .Include(sm => sm.Stemming)
                .ThenInclude(s => s!.StemmingType)
                .Where(sm => sm.MuziekID == muziekId)
                .ToListAsync();
        }

        public async Task<bool> IsAlreadyLinkedAsync(int stemmingId, int muziekId)
        {
            return await _context.Set<StemmingMuziek>()
                .AnyAsync(sm => sm.StemmingID == stemmingId && sm.MuziekID == muziekId);
        }

        public async Task DeleteByStemmingIdAsync(int stemmingId)
        {
            var links = await _context.Set<StemmingMuziek>()
                .Where(sm => sm.StemmingID == stemmingId)
                .ToListAsync();
            
            _context.Set<StemmingMuziek>().RemoveRange(links);
            await _context.SaveChangesAsync();
        }
    }
}
