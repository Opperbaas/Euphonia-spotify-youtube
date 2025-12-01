using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository implementatie voor Muziek
    /// </summary>
    public class MuziekRepository : Repository<Muziek>, IMuziekRepository
    {
        public MuziekRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Muziek>> GetByArtiestAsync(string artiest)
        {
            return await _dbSet
                .Where(m => m.Artiest != null && m.Artiest.Contains(artiest))
                .OrderBy(m => m.Titel)
                .ToListAsync();
        }

        public async Task<IEnumerable<Muziek>> GetByTitelAsync(string titel)
        {
            return await _dbSet
                .Where(m => m.Titel != null && m.Titel.Contains(titel))
                .OrderBy(m => m.Titel)
                .ToListAsync();
        }

        public async Task<Muziek?> GetWithAnalysesAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Analyses)
                .FirstOrDefaultAsync(m => m.MuziekID == id);
        }

        public async Task<IEnumerable<Muziek>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Where(m => 
                    (m.Titel != null && m.Titel.Contains(searchTerm)) ||
                    (m.Artiest != null && m.Artiest.Contains(searchTerm)))
                .OrderBy(m => m.Titel)
                .ToListAsync();
        }
    }
}
