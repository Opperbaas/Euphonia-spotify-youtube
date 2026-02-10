using Xunit;
using Moq;
using Resonance.BusinessLogicLayer.Services;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.DataAccessLayer.Models;
using System.Threading.Tasks;

namespace Resonance.Tests.UnitTests.Services
{
    public class MuziekServiceTests
    {
        [Fact]
        public async Task CreateAsync_SavesYouTubeFields_WhenProvided()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMuziekRepo = new Mock<IMuziekRepository>();
            mockUnitOfWork.Setup(u => u.MuziekRepository).Returns(mockMuziekRepo.Object);

            var dto = new CreateMuziekDto
            {
                Titel = "Test",
                Artiest = "Artist",
                Bron = "YouTube",
                YouTubeVideoId = "dQw4w9WgXcQ",
                ThumbnailUrl = "https://img.youtube.com/vi/dQw4w9WgXcQ/hqdefault.jpg"
            };

            mockMuziekRepo.Setup(r => r.AddAsync(It.IsAny<Muziek>())).ReturnsAsync((Muziek m) => m);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new MuziekService(mockUnitOfWork.Object);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.YouTubeVideoId, result.YouTubeVideoId);
            Assert.Equal(dto.ThumbnailUrl, result.ThumbnailUrl);
            mockMuziekRepo.Verify(r => r.AddAsync(It.IsAny<Muziek>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_SavesSpotifyField_WhenProvided()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMuziekRepo = new Mock<IMuziekRepository>();
            mockUnitOfWork.Setup(u => u.MuziekRepository).Returns(mockMuziekRepo.Object);

            var dto = new CreateMuziekDto
            {
                Titel = "Test Spotify",
                Artiest = "Artist",
                Bron = "Spotify",
                SpotifyTrackId = "3n3Ppam7vgaVa1iaRUc9Lp",
                ThumbnailUrl = "https://i.scdn.co/image/ab67616d0000b273example"
            };

            mockMuziekRepo.Setup(r => r.AddAsync(It.IsAny<Muziek>())).ReturnsAsync((Muziek m) => m);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new MuziekService(mockUnitOfWork.Object);

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.SpotifyTrackId, result.SpotifyTrackId);
            mockMuziekRepo.Verify(r => r.AddAsync(It.IsAny<Muziek>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}

