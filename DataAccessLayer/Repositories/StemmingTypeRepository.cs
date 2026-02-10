using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resonance.DataAccessLayer.Repositories
{
    public class StemmingTypeRepository : Repository<StemmingType>, IStemmingTypeRepository
    {
        public StemmingTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<StemmingType?> GetByNaamAsync(string naam)
        {
            return await _context.Set<StemmingType>()
                .FirstOrDefaultAsync(st => st.Naam != null && st.Naam.ToLower() == naam.ToLower());
        }

        public async Task<IEnumerable<StemmingType>> GetAllTypesAsync()
        {
            return await _context.Set<StemmingType>()
                .OrderBy(st => st.Naam)
                .ToListAsync();
        }
    }
}

