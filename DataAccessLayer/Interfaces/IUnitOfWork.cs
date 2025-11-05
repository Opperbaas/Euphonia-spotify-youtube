using System;
using System.Threading.Tasks;

namespace Euphonia.DataAccessLayer.Interfaces
{
    /// <summary>
    /// Unit of Work pattern voor transactiebeheer
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositories - voeg hier al je repositories toe
        ISpecificRepository SpecificRepository { get; }
        IMuziekRepository MuziekRepository { get; }
        // IYetAnotherRepository YetAnotherRepository { get; }

        // Transactie management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
