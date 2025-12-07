using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Euphonia.PresentationLayer.Controllers;
using Euphonia.BusinessLogicLayer.Interfaces;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.Tests.TestHelpers;

namespace Euphonia.Tests.UnitTests.Controllers
{
    /// <summary>
    /// Unit tests voor ProfielViewController
    /// </summary>
    public class ProfielViewControllerTests
    {
        private readonly Mock<IProfielService> _mockProfielService;
        private readonly ProfielViewController _controller;

        public ProfielViewControllerTests()
        {
            // STAP 1: Mock de service
            _mockProfielService = new Mock<IProfielService>();
            
            // STAP 2: Maak controller met mock service
            _controller = new ProfielViewController(_mockProfielService.Object);
            
            // STAP 3: Setup HttpContext met Session en TempData
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        // Helper: Zet UserId in sessie
        private void SetupUserId(int userId)
        {
            _controller.HttpContext.Session.SetInt32("UserId", userId);
        }

        #region Index Tests

        [Fact]
        public async Task Index_ReturnsViewResult_WithUserProfiles()
        {
            // Arrange
            SetupUserId(1);
            
            var allProfiles = new List<ProfielDto>
            {
                new ProfielDto { ProfielID = 1, UserID = 1, VoorkeurGenres = "Rock" },
                new ProfielDto { ProfielID = 2, UserID = 1, VoorkeurGenres = "Jazz" },
                new ProfielDto { ProfielID = 3, UserID = 2, VoorkeurGenres = "Pop" } // Andere user
            };

            _mockProfielService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(allProfiles);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ProfielDto>>(viewResult.Model);
            
            // Moet alleen profielen van user 1 bevatten
            Assert.Equal(2, model.Count());
            Assert.All(model, p => Assert.Equal(1, p.UserID));
        }

        #endregion

        #region Details Tests

        [Fact]
        public async Task Details_ReturnsViewResult_WhenProfielExists()
        {
            // Arrange
            var testProfiel = new ProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock,Metal",
                Stemmingstags = "Energetic"
            };

            _mockProfielService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(testProfiel);

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfielDto>(viewResult.Model);
            Assert.Equal(1, model.ProfielID);
            Assert.Equal("Rock,Metal", model.VoorkeurGenres);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenProfielDoesNotExist()
        {
            // Arrange
            _mockProfielService.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((ProfielDto?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Create Tests

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_RedirectsToDashboard_WhenModelIsValid()
        {
            // Arrange
            SetupUserId(1);
            
            var createDto = new CreateProfielDto
            {
                VoorkeurGenres = "Rock,Metal",
                Stemmingstags = "Energetic"
            };

            var createdDto = new ProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock,Metal",
                Stemmingstags = "Energetic"
            };

            _mockProfielService.Setup(s => s.CreateAsync(It.IsAny<CreateProfielDto>()))
                .ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Dashboard", redirectResult.ControllerName);
            
            // Verify service was called with correct UserID
            _mockProfielService.Verify(s => s.CreateAsync(It.Is<CreateProfielDto>(
                d => d.UserID == 1
            )), Times.Once);
        }

        [Fact]
        public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            SetupUserId(1);
            var createDto = new CreateProfielDto();
            _controller.ModelState.AddModelError("VoorkeurGenres", "Required");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CreateProfielDto>(viewResult.Model);
            
            // Service should NOT be called
            _mockProfielService.Verify(s => s.CreateAsync(It.IsAny<CreateProfielDto>()), Times.Never);
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task Edit_Get_ReturnsViewResult_WhenProfielExists()
        {
            // Arrange
            var testProfiel = new ProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock",
                Stemmingstags = "Energetic"
            };

            _mockProfielService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(testProfiel);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UpdateProfielDto>(viewResult.Model);
            Assert.Equal(1, model.ProfielID);
            Assert.Equal("Rock", model.VoorkeurGenres);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenProfielDoesNotExist()
        {
            // Arrange
            _mockProfielService.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((ProfielDto?)null);

            // Act
            var result = await _controller.Edit(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToIndex_WhenUpdateSucceeds()
        {
            // Arrange
            var updateDto = new UpdateProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Jazz,Blues",
                Stemmingstags = "Calm"
            };

            var updatedDto = new ProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Jazz,Blues",
                Stemmingstags = "Calm"
            };

            _mockProfielService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProfielDto>()))
                .ReturnsAsync(updatedDto);

            // Act
            var result = await _controller.Edit(updateDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockProfielService.Verify(s => s.UpdateAsync(It.IsAny<UpdateProfielDto>()), Times.Once);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_WhenProfielDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateProfielDto
            {
                ProfielID = 999,
                UserID = 1,
                VoorkeurGenres = "Jazz",
                Stemmingstags = "Calm"
            };

            _mockProfielService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProfielDto>()))
                .ReturnsAsync((ProfielDto?)null);

            // Act
            var result = await _controller.Edit(updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_Get_ReturnsViewResult_WhenProfielExists()
        {
            // Arrange
            var testProfiel = new ProfielDto
            {
                ProfielID = 1,
                UserID = 1,
                VoorkeurGenres = "Rock"
            };

            _mockProfielService.Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(testProfiel);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfielDto>(viewResult.Model);
            Assert.Equal(1, model.ProfielID);
        }

        [Fact]
        public async Task Delete_Get_ReturnsNotFound_WhenProfielDoesNotExist()
        {
            // Arrange
            _mockProfielService.Setup(s => s.GetByIdAsync(999))
                .ReturnsAsync((ProfielDto?)null);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex_AfterDeletion()
        {
            // Arrange
            _mockProfielService.Setup(s => s.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockProfielService.Verify(s => s.DeleteAsync(1), Times.Once);
        }

        #endregion

        #region SetActive Tests

        [Fact]
        public async Task SetActive_RedirectsToIndex_WithSuccessMessage_WhenSucceeds()
        {
            // Arrange
            SetupUserId(1);
            _mockProfielService.Setup(s => s.SetActiveProfielAsync(1, 5))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SetActive(5);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Profiel geactiveerd!", _controller.TempData["SuccessMessage"]);
            _mockProfielService.Verify(s => s.SetActiveProfielAsync(1, 5), Times.Once);
        }

        [Fact]
        public async Task SetActive_RedirectsToIndex_WithErrorMessage_WhenFails()
        {
            // Arrange
            SetupUserId(1);
            _mockProfielService.Setup(s => s.SetActiveProfielAsync(1, 999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SetActive(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Profiel niet gevonden.", _controller.TempData["ErrorMessage"]);
        }

        #endregion
    }
}
