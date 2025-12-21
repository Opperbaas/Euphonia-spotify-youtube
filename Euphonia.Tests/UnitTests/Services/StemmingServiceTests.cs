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
    public class StemmingServiceTests
    {
        [Fact]
        public async Task CreateStemmingAsync_WhenSameTypeExists_AddsMuziekToExisting()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockStemmingRepo = new Mock<IStemmingRepository>();
            var mockStemmingMuziekRepo = new Mock<IStemmingMuziekRepository>();

            mockUnitOfWork.Setup(u => u.StemmingRepository).Returns(mockStemmingRepo.Object);
            mockUnitOfWork.Setup(u => u.StemmingMuziekRepository).Returns(mockStemmingMuziekRepo.Object);

            var existing = new Stemming { StemmingID = 10, UserID = 1, TypeID = 2, Beschrijving = "old", DatumTijd = System.DateTime.Now, StemmingType = new StemmingType { Naam = "TestType" } };
            mockStemmingRepo.Setup(r => r.GetLatestByUserAndTypeAsync(1, 2)).ReturnsAsync(existing);
            mockStemmingRepo.Setup(r => r.GetStemmingMetDetailsAsync(10)).ReturnsAsync(existing);

            // No existing links
            mockStemmingMuziekRepo.Setup(r => r.IsAlreadyLinkedAsync(10, 101)).ReturnsAsync(false);
            mockStemmingMuziekRepo.Setup(r => r.IsAlreadyLinkedAsync(10, 102)).ReturnsAsync(false);

            // Simulate GetMuziekByStemmingIdAsync returning the two linked muziek
            var links = new List<StemmingMuziek>
            {
                new StemmingMuziek { StemmingID = 10, MuziekID = 101, Muziek = new Muziek { MuziekID = 101, Titel = "Song1" } },
                new StemmingMuziek { StemmingID = 10, MuziekID = 102, Muziek = new Muziek { MuziekID = 102, Titel = "Song2" } },
            };
            mockStemmingMuziekRepo.Setup(r => r.GetMuziekByStemmingIdAsync(10)).ReturnsAsync(links);

            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var service = new StemmingService(mockUnitOfWork.Object);

            var createDto = new CreateStemmingDto
            {
                TypeID = 2,
                MuziekIDs = new List<int> { 101, 102 },
                Beschrijving = "newdesc"
            };

            // Act
            var result = await service.CreateStemmingAsync(createDto, 1);

            // Assert
            // Verify that new links were added for each muziek
            mockStemmingMuziekRepo.Verify(r => r.AddAsync(It.Is<StemmingMuziek>(s => s.MuziekID == 101 && s.StemmingID == 10)), Times.Once);
            mockStemmingMuziekRepo.Verify(r => r.AddAsync(It.Is<StemmingMuziek>(s => s.MuziekID == 102 && s.StemmingID == 10)), Times.Once);

            // Verify that the existing stemming was updated (description appended)
            mockStemmingRepo.Verify(r => r.UpdateAsync(It.IsAny<Stemming>()), Times.AtLeastOnce);

            // Verify SaveChanges was called
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);

            Assert.NotNull(result);
            Assert.Equal(2, result.GekoppeldeMuziek.Count);
            Assert.Contains(result.GekoppeldeMuziek, m => m.MuziekID == 101);
            Assert.Contains(result.GekoppeldeMuziek, m => m.MuziekID == 102);
        }
    }
}
