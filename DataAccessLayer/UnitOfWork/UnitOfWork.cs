using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Repositories;

namespace Euphonia.DataAccessLayer.UnitOfWork
{
    /// <summary>
    /// Unit of Work implementatie voor transactiebeheer
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        // Repositories
        private ISpecificRepository _specificRepository;
        private IMuziekRepository _muziekRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Repository properties met lazy initialization
        public ISpecificRepository SpecificRepository
        {
            get
            {
                if (_specificRepository == null)
                {
                    _specificRepository = new SpecificRepository(_context);
                }
                return _specificRepository;
            }
        }

        public IMuziekRepository MuziekRepository
        {
            get
            {
                if (_muziekRepository == null)
                {
                    _muziekRepository = new MuziekRepository(_context);
                }
                return _muziekRepository;
            }
        }

        // Voeg hier meer repositories toe
        // public IAnotherRepository AnotherRepository
        // {
        //     get
        //     {
        //         if (_anotherRepository == null)
        //         {
        //             _anotherRepository = new AnotherRepository(_context);
        //         }
        //         return _anotherRepository;
        //     }
        // }

        // Transactie management
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // Dispose pattern
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
