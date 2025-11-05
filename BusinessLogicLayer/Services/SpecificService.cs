using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service implementatie - bevat business logic
    /// </summary>
    public class SpecificService : ISpecificService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecificService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // READ operaties
        public async Task<SpecificDto> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.SpecificRepository.GetByIdAsync(id);
            
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }

        public async Task<IEnumerable<SpecificDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.SpecificRepository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<IEnumerable<SpecificDto>> GetActiveItemsAsync()
        {
            var entities = await _unitOfWork.SpecificRepository.GetActiveItemsAsync();
            return entities.Select(MapToDto);
        }

        public async Task<IEnumerable<SpecificDto>> SearchByNameAsync(string name)
        {
            // Validatie
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty", nameof(name));
            }

            var entities = await _unitOfWork.SpecificRepository.GetByNameAsync(name);
            return entities.Select(MapToDto);
        }

        // CREATE
        public async Task<SpecificDto> CreateAsync(CreateSpecificDto dto)
        {
            // Validatie
            ValidateCreateDto(dto);

            // Business logic
            var entity = new SpecificEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.SpecificRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        // UPDATE
        public async Task<SpecificDto> UpdateAsync(UpdateSpecificDto dto)
        {
            // Validatie
            ValidateUpdateDto(dto);

            var entity = await _unitOfWork.SpecificRepository.GetByIdAsync(dto.Id);
            
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with Id {dto.Id} not found");
            }

            // Business logic - update alleen wat nodig is
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedDate = DateTime.Now;

            await _unitOfWork.SpecificRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(entity);
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SpecificRepository.GetByIdAsync(id);
            
            if (entity == null)
            {
                return false;
            }

            // Business logic - bijv. check of item verwijderd mag worden
            // if (!CanBeDeleted(entity))
            // {
            //     throw new InvalidOperationException("Cannot delete this item");
            // }

            await _unitOfWork.SpecificRepository.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Business logic methods
        public async Task<bool> ActivateAsync(int id)
        {
            var entity = await _unitOfWork.SpecificRepository.GetByIdAsync(id);
            
            if (entity == null)
            {
                return false;
            }

            entity.IsActive = true;
            entity.ModifiedDate = DateTime.Now;

            await _unitOfWork.SpecificRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var entity = await _unitOfWork.SpecificRepository.GetByIdAsync(id);
            
            if (entity == null)
            {
                return false;
            }

            entity.IsActive = false;
            entity.ModifiedDate = DateTime.Now;

            await _unitOfWork.SpecificRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetActiveCountAsync()
        {
            var entities = await _unitOfWork.SpecificRepository.GetActiveItemsAsync();
            return entities.Count();
        }

        // Private helper methods
        private SpecificDto MapToDto(SpecificEntity entity)
        {
            return new SpecificDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate
            };
        }

        private void ValidateCreateDto(CreateSpecificDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Name is required", nameof(dto.Name));
            }

            if (dto.Name.Length > 100)
            {
                throw new ArgumentException("Name cannot exceed 100 characters", nameof(dto.Name));
            }

            // Voeg meer validatie toe
        }

        private void ValidateUpdateDto(UpdateSpecificDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.Id <= 0)
            {
                throw new ArgumentException("Invalid Id", nameof(dto.Id));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Name is required", nameof(dto.Name));
            }

            if (dto.Name.Length > 100)
            {
                throw new ArgumentException("Name cannot exceed 100 characters", nameof(dto.Name));
            }

            // Voeg meer validatie toe
        }
    }
}
