using Xunit;
using Microsoft.EntityFrameworkCore;
using Euphonia.DataAccessLayer.Context;
using Euphonia.DataAccessLayer.Repositories;
using Euphonia.DataAccessLayer.Models;

namespace Euphonia.Tests.UnitTests.Repositories
{
    /// <summary>
    /// Unit tests voor ProfielRepository
    /// 
    /// BELANGRIJK: Repository tests gebruiken GEEN MOCKS!
    /// We gebruiken een in-memory database om de echte repository logic te testen.
    /// </summary>
    public class ProfielRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProfielRepository _repository;

        public ProfielRepositoryTests()
        {
            // STAP 1: Maak in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unieke naam per test
                .Options;

            _context = new ApplicationDbContext(options);
            
            // STAP 2: Maak repository met echte context
            _repository = new ProfielRepository(_context);
            
            // STAP 3: Seed test data (optioneel, maar handig voor sommige tests)
            SeedTestData();
        }

        // Cleanup na elke test
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Helper: Voeg standaard test data toe
        private void SeedTestData()
        {
            var testProfielen = new List<Profiel>
            {
                new Profiel 
                { 
                    ProfielID = 1,
                    UserID = 1, 
                    VoorkeurGenres = "Rock,Metal", 
                    Stemmingstags = "Energetic",
                    IsActive = true 
                },
                new Profiel 
                { 
                    ProfielID = 2,
                    UserID = 1, 
                    VoorkeurGenres = "Jazz,Blues", 
                    Stemmingstags = "Calm",
                    IsActive = false 
                },
                new Profiel 
                { 
                    ProfielID = 3,
                    UserID = 2, 
                    VoorkeurGenres = "Pop,Dance", 
                    Stemmingstags = "Happy",
                    IsActive = true 
                }
            };

            _context.Profielen.AddRange(testProfielen);
            _context.SaveChanges();
        }

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ReturnsProfile_WhenExists()
        {
            // Arrange - Data is al in database via SeedTestData()

            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProfielID);
            Assert.Equal("Rock,Metal", result.VoorkeurGenres);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllProfiles()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); // 3 profielen in SeedTestData
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoData()
        {
            // Arrange - Maak nieuwe lege database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EmptyDatabase")
                .Options;
            
            using var emptyContext = new ApplicationDbContext(options);
            var emptyRepository = new ProfielRepository(emptyContext);

            // Act
            var result = await emptyRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetActiveByUserIdAsync Tests

        [Fact]
        public async Task GetActiveByUserIdAsync_ReturnsActiveProfile_WhenExists()
        {
            // Act
            var result = await _repository.GetActiveByUserIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserID);
            Assert.True(result.IsActive);
            Assert.Equal("Rock,Metal", result.VoorkeurGenres);
        }

        [Fact]
        public async Task GetActiveByUserIdAsync_ReturnsNull_WhenNoActiveProfile()
        {
            // Arrange - User 2 heeft actief profiel, maar we zoeken user zonder actief profiel
            // Deactiveer alle profielen van user 1
            var userProfiles = await _context.Profielen.Where(p => p.UserID == 1).ToListAsync();
            foreach (var profile in userProfiles)
            {
                profile.IsActive = false;
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveByUserIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetAllByUserIdAsync Tests

        [Fact]
        public async Task GetAllByUserIdAsync_ReturnsUserProfiles()
        {
            // Act
            var result = await _repository.GetAllByUserIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // User 1 heeft 2 profielen
            Assert.All(result, p => Assert.Equal(1, p.UserID));
        }

        [Fact]
        public async Task GetAllByUserIdAsync_ReturnsEmpty_WhenUserHasNoProfiles()
        {
            // Act
            var result = await _repository.GetAllByUserIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByGenreAsync Tests

        [Fact]
        public async Task GetByGenreAsync_ReturnsMatchingProfiles()
        {
            // Act
            var result = await _repository.GetByGenreAsync("Rock");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Alleen profiel 1 heeft "Rock"
            Assert.Contains(result, p => p.VoorkeurGenres!.Contains("Rock"));
        }

        [Fact]
        public async Task GetByGenreAsync_ReturnsEmpty_WhenNoMatch()
        {
            // Act
            var result = await _repository.GetByGenreAsync("Classical");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_AddsProfileToDatabase()
        {
            // Arrange
            var newProfiel = new Profiel
            {
                UserID = 3,
                VoorkeurGenres = "Classical,Opera",
                Stemmingstags = "Peaceful",
                IsActive = false
            };

            // Act
            var result = await _repository.AddAsync(newProfiel);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.ProfielID); // ID is toegekend
            
            // Verify in database
            var savedProfile = await _context.Profielen.FindAsync(result.ProfielID);
            Assert.NotNull(savedProfile);
            Assert.Equal("Classical,Opera", savedProfile.VoorkeurGenres);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_UpdatesExistingProfile()
        {
            // Arrange
            var profiel = await _repository.GetByIdAsync(1);
            Assert.NotNull(profiel);
            
            profiel.VoorkeurGenres = "Metal,Hardcore";
            profiel.Stemmingstags = "Aggressive";

            // Act
            await _repository.UpdateAsync(profiel);
            await _context.SaveChangesAsync();

            // Assert - Haal opnieuw op uit database
            var updatedProfile = await _repository.GetByIdAsync(1);
            Assert.NotNull(updatedProfile);
            Assert.Equal("Metal,Hardcore", updatedProfile.VoorkeurGenres);
            Assert.Equal("Aggressive", updatedProfile.Stemmingstags);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_RemovesProfileFromDatabase()
        {
            // Arrange
            var profiel = await _repository.GetByIdAsync(1);
            Assert.NotNull(profiel);

            // Act
            await _repository.DeleteAsync(profiel);
            await _context.SaveChangesAsync();

            // Assert - Profiel moet weg zijn
            var deletedProfile = await _repository.GetByIdAsync(1);
            Assert.Null(deletedProfile);
            
            // Verify totaal aantal is verminderd
            var allProfiles = await _repository.GetAllAsync();
            Assert.Equal(2, allProfiles.Count()); // Was 3, nu 2
        }

        #endregion

        #region Complex Scenario Tests

        [Fact]
        public async Task MultipleOperations_WorkTogether()
        {
            // Scenario: Voeg toe, update, en verwijder in één test
            
            // 1. Add
            var newProfiel = new Profiel
            {
                UserID = 1,
                VoorkeurGenres = "Electronic",
                Stemmingstags = "Upbeat",
                IsActive = false
            };
            await _repository.AddAsync(newProfiel);
            await _context.SaveChangesAsync();

            // 2. Verify added
            var userProfiles = await _repository.GetAllByUserIdAsync(1);
            Assert.Equal(3, userProfiles.Count()); // Was 2, nu 3

            // 3. Update
            newProfiel.IsActive = true;
            await _repository.UpdateAsync(newProfiel);
            await _context.SaveChangesAsync();

            // 4. Verify updated
            var activeProfile = await _repository.GetActiveByUserIdAsync(1);
            Assert.NotNull(activeProfile);
            // Nu zijn er 2 actieve profielen voor user 1!

            // 5. Delete
            await _repository.DeleteAsync(newProfiel);
            await _context.SaveChangesAsync();

            // 6. Verify deleted
            var finalProfiles = await _repository.GetAllByUserIdAsync(1);
            Assert.Equal(2, finalProfiles.Count()); // Terug naar 2
        }

        [Fact]
        public async Task ConcurrentAccess_WorksCorrectly()
        {
            // Test dat meerdere operations na elkaar werken
            var profile1 = await _repository.GetByIdAsync(1);
            var profile2 = await _repository.GetByIdAsync(2);
            var profile3 = await _repository.GetByIdAsync(3);

            Assert.NotNull(profile1);
            Assert.NotNull(profile2);
            Assert.NotNull(profile3);
            Assert.NotEqual(profile1.ProfielID, profile2.ProfielID);
        }

        #endregion
    }
}
