using Microsoft.EntityFrameworkCore;
using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Repositories
{
    /// <summary>
    /// Repository voor User data access
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Set<User>()
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
