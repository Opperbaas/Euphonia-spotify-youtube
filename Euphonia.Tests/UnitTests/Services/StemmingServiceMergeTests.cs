using Xunit;
using Moq;
using Euphonia.BusinessLogicLayer.Services;
using Euphonia.DataAccessLayer.Interfaces;
using Euphonia.DataAccessLayer.Repositories;
using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.DataAccessLayer.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Euphonia.Tests.UnitTests.Services
{
    public class StemmingServiceMergeTests
    {
        [Fact]
        public async Task CreateStemmingAsync_WhenNoExisting_CreatesNewStemming()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStemmingRepo = new Mock<IStemmingRepository>();
            var mockStemmingMuziekRepo = new Mock<IStemmingMuziekRepository>();

            mockUnitOfWork.Setup(u => u.StemmingRepository).Returns(mockStemmingRepo.Object);
            mockUnitOfWork.Setup(u => u.StemmingMuziekRepository).Returns(mockStemmingMuziekRepo.Object);

            mockStemmingRepo.Setup(r => r.GetLatestByUserAndTypeAsync(1, 2)).ReturnsAsync((Stemming?)null);

            mockStemmingRepo.Setup(r => r.AddAsync(It.IsAny<Stemming>())).ReturnsAsync((Stemming s) => { s.StemmingID = 20; s.DatumTijd = System.DateTime.Now; return s; });

            // Ensure the repository returns the saved stemming with details when requested
            var saved = new Stemming { StemmingID = 20, UserID = 1, TypeID = 2, Beschrijving = "first", DatumTijd = System.DateTime.Now, StemmingType = new StemmingType { Naam = "TestType" } };
            mockStemmingRepo.Setup(r => r.GetStemmingMetDetailsAsync(20)).ReturnsAsync(saved);

            // Ensure GetMuziekByStemmingIdAsync returns the linked muziek
            var muziekLinks = new List<StemmingMuziek>
            {
                new StemmingMuziek { StemmingID = 20, MuziekID = 201, Muziek = new Muziek { MuziekID = 201, Titel = "Song201" } }
            };
            mockStemmingMuziekRepo.Setup(r => r.GetMuziekByStemmingIdAsync(20)).ReturnsAsync(muziekLinks);

            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new StemmingService(mockUnitOfWork.Object);

            var createDto = new CreateStemmingDto
            {
                TypeID = 2,
                MuziekIDs = new List<int> { 201 },
                Beschrijving = "first"
            };

            // Act
            var result = await service.CreateStemmingAsync(createDto, 1);

            // Assert
            mockStemmingRepo.Verify(r => r.AddAsync(It.IsAny<Stemming>()), Times.Once);
            mockStemmingMuziekRepo.Verify(r => r.AddAsync(It.IsAny<StemmingMuziek>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(1, result.GekoppeldeMuziek.Count);
            Assert.Equal(201, result.GekoppeldeMuziek.First().MuziekID);
        }

        [Fact]
        public async Task CreateStemmingAsync_WhenExisting_DoesNotDuplicateLinks()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStemmingRepo = new Mock<IStemmingRepository>();
            var mockStemmingMuziekRepo = new Mock<IStemmingMuziekRepository>();

            mockUnitOfWork.Setup(u => u.StemmingRepository).Returns(mockStemmingRepo.Object);
            mockUnitOfWork.Setup(u => u.StemmingMuziekRepository).Returns(mockStemmingMuziekRepo.Object);

            var existing = new Stemming { StemmingID = 30, UserID = 1, TypeID = 2, Beschrijving = "old", DatumTijd = System.DateTime.Now, StemmingType = new StemmingType { Naam = "TestType" } };
            mockStemmingRepo.Setup(r => r.GetLatestByUserAndTypeAsync(1, 2)).ReturnsAsync(existing);
            mockStemmingRepo.Setup(r => r.GetStemmingMetDetailsAsync(30)).ReturnsAsync(existing);

            // Ensure GetMuziekByStemmingIdAsync returns current links (include the existing 301) and the newly added 302
            var existingLinks = new List<StemmingMuziek>
            {
                new StemmingMuziek { StemmingID = 30, MuziekID = 301, Muziek = new Muziek { MuziekID = 301, Titel = "Song301" } },
                new StemmingMuziek { StemmingID = 30, MuziekID = 302, Muziek = new Muziek { MuziekID = 302, Titel = "Song302" } }
            };
            mockStemmingMuziekRepo.Setup(r => r.GetMuziekByStemmingIdAsync(30)).ReturnsAsync(existingLinks);

            // Existing linked muziek: 301
            mockStemmingMuziekRepo.Setup(r => r.IsAlreadyLinkedAsync(30, 301)).ReturnsAsync(true);
            mockStemmingMuziekRepo.Setup(r => r.IsAlreadyLinkedAsync(30, 302)).ReturnsAsync(false);

            mockStemmingMuziekRepo.Setup(r => r.AddAsync(It.IsAny<StemmingMuziek>())).ReturnsAsync((StemmingMuziek m) => m);
            mockStemmingRepo.Setup(r => r.UpdateAsync(It.IsAny<Stemming>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new StemmingService(mockUnitOfWork.Object);

            var createDto = new CreateStemmingDto
            {
                TypeID = 2,
                MuziekIDs = new List<int> { 301, 302 },
                Beschrijving = "added"
            };

            // Act
            var result = await service.CreateStemmingAsync(createDto, 1);

            // Assert
            // 301 should not be added again, 302 should be added once
            mockStemmingMuziekRepo.Verify(r => r.AddAsync(It.Is<StemmingMuziek>(s => s.MuziekID == 301)), Times.Never);
            mockStemmingMuziekRepo.Verify(r => r.AddAsync(It.Is<StemmingMuziek>(s => s.MuziekID == 302)), Times.Once);

            mockStemmingRepo.Verify(r => r.UpdateAsync(It.IsAny<Stemming>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);

            Assert.NotNull(result);
            Assert.Contains(result.GekoppeldeMuziek, m => m.MuziekID == 301);
            Assert.Contains(result.GekoppeldeMuziek, m => m.MuziekID == 302);
        }

        [Fact]
        public async Task CreateStemmingAsync_AppendsDescription_WhenExisting()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStemmingRepo = new Mock<IStemmingRepository>();
            var mockStemmingMuziekRepo = new Mock<IStemmingMuziekRepository>();

            mockUnitOfWork.Setup(u => u.StemmingRepository).Returns(mockStemmingRepo.Object);
            mockUnitOfWork.Setup(u => u.StemmingMuziekRepository).Returns(mockStemmingMuziekRepo.Object);

            var existing = new Stemming { StemmingID = 40, UserID = 1, TypeID = 2, Beschrijving = "old", DatumTijd = System.DateTime.Now, StemmingType = new StemmingType { Naam = "TestType" } };
            mockStemmingRepo.Setup(r => r.GetLatestByUserAndTypeAsync(1, 2)).ReturnsAsync(existing);
            mockStemmingRepo.Setup(r => r.GetStemmingMetDetailsAsync(40)).ReturnsAsync(existing);

            mockStemmingMuziekRepo.Setup(r => r.IsAlreadyLinkedAsync(40, 401)).ReturnsAsync(false);
            mockStemmingMuziekRepo.Setup(r => r.AddAsync(It.IsAny<StemmingMuziek>())).ReturnsAsync((StemmingMuziek m) => m);

            // Ensure GetMuziekByStemmingIdAsync returns updated list with the newly added muziek
            var updatedLinks = new List<StemmingMuziek>
            {
                new StemmingMuziek { StemmingID = 40, MuziekID = 401, Muziek = new Muziek { MuziekID = 401, Titel = "Song401" } }
            };
            mockStemmingMuziekRepo.Setup(r => r.GetMuziekByStemmingIdAsync(40)).ReturnsAsync(updatedLinks);

            mockStemmingRepo.Setup(r => r.UpdateAsync(It.IsAny<Stemming>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new StemmingService(mockUnitOfWork.Object);

            var createDto = new CreateStemmingDto
            {
                TypeID = 2,
                MuziekIDs = new List<int> { 401 },
                Beschrijving = "more"
            };

            // Act
            var result = await service.CreateStemmingAsync(createDto, 1);

            // Assert
            mockStemmingRepo.Verify(r => r.UpdateAsync(It.Is<Stemming>(s => s.Beschrijving != null && s.Beschrijving.Contains("old") && s.Beschrijving.Contains("more"))), Times.Once);
            mockStemmingMuziekRepo.Verify(r => r.AddAsync(It.IsAny<StemmingMuziek>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        }
    }
}
