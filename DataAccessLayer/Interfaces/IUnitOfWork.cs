using System;
using System.Threading.Tasks;
using Euphonia.DataAccessLayer.Repositories;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Unit of Work pattern voor transactiebeheer
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IMuziekRepository MuziekRepository { get; }
        IProfielRepository ProfielRepository { get; }
        IUserRepository UserRepository { get; }
        IStemmingRepository StemmingRepository { get; }
        IStemmingTypeRepository StemmingTypeRepository { get; }
        IStemmingMuziekRepository StemmingMuziekRepository { get; }
        INotificatieRepository NotificatieRepository { get; }

        // Transactie management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
