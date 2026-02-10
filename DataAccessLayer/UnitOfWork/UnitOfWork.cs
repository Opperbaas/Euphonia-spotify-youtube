using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Repositories;

namespace Resonance.DataAccessLayer.UnitOfWork
{
    /// <summary>
    /// Unit of Work implementatie voor transactiebeheer
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repositories
        private IMuziekRepository? _muziekRepository;
        private IProfielRepository? _profielRepository;
        private IUserRepository? _userRepository;
        private IStemmingRepository? _stemmingRepository;
        private IStemmingTypeRepository? _stemmingTypeRepository;
        private IStemmingMuziekRepository? _stemmingMuziekRepository;
        private INotificatieRepository? _notificatieRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
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

        public IProfielRepository ProfielRepository
        {
            get
            {
                if (_profielRepository == null)
                {
                    _profielRepository = new ProfielRepository(_context);
                }
                return _profielRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public IStemmingRepository StemmingRepository
        {
            get
            {
                if (_stemmingRepository == null)
                {
                    _stemmingRepository = new StemmingRepository(_context);
                }
                return _stemmingRepository;
            }
        }

        public IStemmingTypeRepository StemmingTypeRepository
        {
            get
            {
                if (_stemmingTypeRepository == null)
                {
                    _stemmingTypeRepository = new StemmingTypeRepository(_context);
                }
                return _stemmingTypeRepository;
            }
        }

        public IStemmingMuziekRepository StemmingMuziekRepository
        {
            get
            {
                if (_stemmingMuziekRepository == null)
                {
                    _stemmingMuziekRepository = new StemmingMuziekRepository(_context);
                }
                return _stemmingMuziekRepository;
            }
        }

        public INotificatieRepository NotificatieRepository
        {
            get
            {
                if (_notificatieRepository == null)
                {
                    _notificatieRepository = new NotificatieRepository(_context);
                }
                return _notificatieRepository;
            }
        }

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

