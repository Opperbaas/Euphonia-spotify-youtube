using Euphonia.DataAccessLayer.Models;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Repository interface voor User
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }
}
