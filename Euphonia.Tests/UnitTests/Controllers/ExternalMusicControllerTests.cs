using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Euphonia.PresentationLayer.Controllers;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.BusinessLogicLayer.DTOs;
using System.Threading.Tasks;

namespace Euphonia.Tests.UnitTests.Controllers
{
    public class ExternalMusicControllerTests
    {
        [Fact]
        public async Task GetYouTubeVideo_ReturnsOk_WhenVideoFound()
        {
            // Arrange
            var mockSpotify = new Mock<ISpotifyService>();
            var mockYT = new Mock<IYouTubeService>();
            var dto = new ExternalMusicSearchResultDto
            {
                ExternalId = "dQw4w9WgXcQ",
                Titel = "Test",
                Artiest = "Artist",
                AlbumImageUrl = "https://example.com/thumb.jpg",
                ExternalUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                Platform = "YouTube"
            };

            mockYT.Setup(s => s.GetVideoMetadataAsync(It.IsAny<string>())).ReturnsAsync(dto);
            var controller = new ExternalMusicController(mockSpotify.Object, mockYT.Object);

            // Act
            var result = await controller.GetYouTubeVideo("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetYouTubeVideo_ReturnsNotFound_WhenNull()
        {
            var mockSpotify = new Mock<ISpotifyService>();
            var mockYT = new Mock<IYouTubeService>();
            mockYT.Setup(s => s.GetVideoMetadataAsync(It.IsAny<string>())).ReturnsAsync((ExternalMusicSearchResultDto?)null);
            var controller = new ExternalMusicController(mockSpotify.Object, mockYT.Object);

            var result = await controller.GetYouTubeVideo("https://www.youtube.com/watch?v=invalid");
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
