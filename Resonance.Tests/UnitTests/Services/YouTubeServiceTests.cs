using Xunit;
using Microsoft.Extensions.Configuration;
using Resonance.BusinessLogicLayer.Services;
using System.Collections.Generic;

namespace Resonance.Tests.UnitTests.Services
{
    public class YouTubeServiceTests
    {
        private readonly YouTubeService _service;

        public YouTubeServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
                { "ExternalMusicApis:YouTube:ApiKey", "TEST_KEY" }
            };
            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            _service = new YouTubeService(config);
        }

        [Theory]
        [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        [InlineData("https://youtu.be/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        [InlineData("https://www.youtube.com/embed/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        [InlineData("dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        public void ExtractVideoId_Returns_CorrectId(string input, string expected)
        {
            var result = _service.ExtractVideoId(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("https://example.com/watch?v=abc")]
        public void ExtractVideoId_ReturnsNull_ForInvalid(string input)
        {
            var result = _service.ExtractVideoId(input);
            Assert.Null(result);
        }
    }
}

