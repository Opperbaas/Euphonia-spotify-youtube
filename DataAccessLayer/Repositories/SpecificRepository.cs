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
    /// Specifieke repository implementatie
    /// </summary>
    public class SpecificRepository : Repository<SpecificEntity>, ISpecificRepository
    {
        public SpecificRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Custom method implementaties
        public async Task<IEnumerable<SpecificEntity>> GetActiveItemsAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpecificEntity>> GetByNameAsync(string name)
        {
            return await _dbSet
                .Where(x => x.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<SpecificEntity> GetWithRelatedDataAsync(int id)
        {
            return await _dbSet
                // .Include(x => x.RelatedEntity)  // Eager loading
                // .ThenInclude(x => x.ChildEntity)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // Voeg hier je eigen custom methods toe
    }
}
