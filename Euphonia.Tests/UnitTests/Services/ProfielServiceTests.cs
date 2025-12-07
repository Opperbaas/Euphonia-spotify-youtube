using Xunit;
using Moq;
using Euphonia.BusinessLogicLayer.Services;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Models;
using Euphonia.BusinessLogicLayer.DTOs;

namespace Euphonia.Tests.UnitTests.Services
{
    public class ProfielServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProfielRepository> _mockProfielRepository;
        private readonly ProfielService _service;

        public ProfielServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProfielRepository = new Mock<IProfielRepository>();
            _mockUnitOfWork.Setup(u => u.ProfielRepository).Returns(_mockProfielRepository.Object);
            _service = new ProfielService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProfielDto_WhenProfielExists()
        {
            // Arrange - Test data voorbereiden
            var testProfiel = new Profiel
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock,Pop",
                Stemmingstags = "Happy,Energetic",
                IsActive = true
            };

            _mockProfielRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(testProfiel);

            // Act - Methode uitvoeren
            var result = await _service.GetByIdAsync(1);

            // Assert - Resultaat controleren
            Assert.NotNull(result);
            Assert.Equal(1, result.ProfielID);
            Assert.Equal("Rock,Pop", result.VoorkeurGenres);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenProfielDoesNotExist()
        {
            // Arrange
            _mockProfielRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Profiel?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetActiveProfielAsync_ReturnsActiveProfile_WhenExists()
        {
            // Arrange
            var activeProfiel = new Profiel
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock",
                Stemmingstags = "Energetic",
                IsActive = true
            };

            _mockProfielRepository.Setup(r => r.GetActiveByUserIdAsync(1))
                .ReturnsAsync(activeProfiel);

            // Act
            var result = await _service.GetActiveProfielAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsActive);
            Assert.Equal(1, result.ProfielID);
        }

        [Fact]
        public async Task SetActiveProfielAsync_ActivatesCorrectProfile_AndDeactivatesOthers()
        {
            // Arrange
            var userProfiles = new List<Profiel>
            {
                new Profiel { ProfielID = 1, UserID = 1, VoorkeurGenres = "Rock", IsActive = true },
                new Profiel { ProfielID = 2, UserID = 1, VoorkeurGenres = "Jazz", IsActive = false },
                new Profiel { ProfielID = 3, UserID = 1, VoorkeurGenres = "Pop", IsActive = false }
            };

            _mockProfielRepository.Setup(r => r.GetAllByUserIdAsync(1))
                .ReturnsAsync(userProfiles);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act - Activeer profiel 2
            var result = await _service.SetActiveProfielAsync(1, 2);

            // Assert
            Assert.True(result);
            Assert.False(userProfiles[0].IsActive); // Profiel 1 gedeactiveerd
            Assert.True(userProfiles[1].IsActive);  // Profiel 2 geactiveerd
            Assert.False(userProfiles[2].IsActive); // Profiel 3 blijft gedeactiveerd
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedProfiel_WhenValidDataProvided()
        {
            // Arrange
            var createDto = new CreateProfielDto
            {
                UserID = 1,
                VoorkeurGenres = "Rock,Metal",
                Stemmingstags = "Energetic",
                IsActive = true
            };

            _mockProfielRepository.Setup(r => r.AddAsync(It.IsAny<Profiel>()))
                .Callback<Profiel>(p => p.ProfielID = 1) // Simuleer database die ID toekent
                .ReturnsAsync((Profiel p) => p);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProfielID);
            Assert.Equal("Rock,Metal", result.VoorkeurGenres);
            Assert.Equal("Energetic", result.Stemmingstags);
            _mockProfielRepository.Verify(r => r.AddAsync(It.IsAny<Profiel>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesProfiel_WhenProfielExists()
        {
            // Arrange
            var updateDto = new UpdateProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Classical,Opera",
                Stemmingstags = "Calm,Peaceful",
                IsActive = false
            };

            var existingProfiel = new Profiel
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock",
                Stemmingstags = "Energetic",
                IsActive = true
            };

            _mockProfielRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProfiel);
            _mockProfielRepository.Setup(r => r.UpdateAsync(It.IsAny<Profiel>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Classical,Opera", result.VoorkeurGenres);
            Assert.Equal("Calm,Peaceful", result.Stemmingstags);
            _mockProfielRepository.Verify(r => r.UpdateAsync(It.IsAny<Profiel>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenProfielExists()
        {
            // Arrange
            var existingProfiel = new Profiel
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock",
                Stemmingstags = "Energetic",
                IsActive = true
            };

            _mockProfielRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProfiel);
            _mockProfielRepository.Setup(r => r.DeleteAsync(It.IsAny<Profiel>()))
                .Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockProfielRepository.Verify(r => r.DeleteAsync(It.IsAny<Profiel>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenProfielDoesNotExist()
        {
            // Arrange
            _mockProfielRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Profiel?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockProfielRepository.Verify(r => r.DeleteAsync(It.IsAny<Profiel>()), Times.Never);
        }
    }
}
