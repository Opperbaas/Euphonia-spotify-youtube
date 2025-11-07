using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Euphonia.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service voor YouTube Data API integratie
    /// </summary>
    public class YouTubeService : IYouTubeService
    {
        private readonly string _apiKey;
        private Google.Apis.YouTube.v3.YouTubeService? _youtubeApiService;

        public YouTubeService(IConfiguration configuration)
        {
            _apiKey = configuration["ExternalMusicApis:YouTube:ApiKey"] ?? "";
        }

        /// <summary>
        /// Initialiseert YouTube service
        /// </summary>
        private Google.Apis.YouTube.v3.YouTubeService GetYouTubeApiService()
        {
            if (_youtubeApiService != null)
                return _youtubeApiService;

            if (string.IsNullOrEmpty(_apiKey) || _apiKey == "YOUR_YOUTUBE_API_KEY")
            {
                throw new InvalidOperationException(
                    "YouTube API key is niet ingesteld in appsettings.json. " +
                    "Bezoek https://console.cloud.google.com voor het aanmaken van een API key."
                );
            }

            _youtubeApiService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = "Euphonia"
            });

            return _youtubeApiService;
        }

        /// <summary>
        /// Extraheert video ID uit YouTube URL
        /// Ondersteunt formaten zoals:
        /// - https://www.youtube.com/watch?v=VIDEO_ID
        /// - https://youtu.be/VIDEO_ID
        /// - https://www.youtube.com/embed/VIDEO_ID
        /// </summary>
        public string? ExtractVideoId(string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
                return null;

            // Patroon voor verschillende YouTube URL formaten
            var patterns = new[]
            {
                @"(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)([a-zA-Z0-9_-]{11})",
                @"^([a-zA-Z0-9_-]{11})$" // Direct video ID
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(videoUrl, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Haalt metadata op van een YouTube video
        /// </summary>
        public async Task<ExternalMusicSearchResultDto?> GetVideoMetadataAsync(string videoUrl)
        {
            try
            {
                var videoId = ExtractVideoId(videoUrl);
                if (string.IsNullOrEmpty(videoId))
                {
                    return null;
                }

                var youtube = GetYouTubeApiService();
                var request = youtube.Videos.List("snippet,contentDetails");
                request.Id = videoId;

                var response = await request.ExecuteAsync();
                var video = response.Items?.FirstOrDefault();

                if (video == null)
                    return null;

                // Parse duration (ISO 8601 format zoals PT4M33S)
                var duration = ParseDuration(video.ContentDetails?.Duration);

                return new ExternalMusicSearchResultDto
                {
                    ExternalId = videoId,
                    Titel = video.Snippet?.Title ?? "Onbekende titel",
                    Artiest = video.Snippet?.ChannelTitle ?? "Onbekende artiest",
                    Platform = "YouTube",
                    AlbumImageUrl = video.Snippet?.Thumbnails?.High?.Url 
                                    ?? video.Snippet?.Thumbnails?.Default__?.Url,
                    PreviewUrl = null, // YouTube heeft geen audio preview
                    ExternalUrl = $"https://www.youtube.com/watch?v={videoId}",
                    DurationMs = duration
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"YouTube API Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Parsed ISO 8601 duration naar milliseconden
        /// </summary>
        private int? ParseDuration(string? duration)
        {
            if (string.IsNullOrEmpty(duration))
                return null;

            try
            {
                // Voorbeeld: PT4M33S = 4 minuten 33 seconden
                var match = Regex.Match(duration, @"PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+)S)?");
                if (!match.Success)
                    return null;

                int hours = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
                int minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
                int seconds = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

                return (hours * 3600 + minutes * 60 + seconds) * 1000;
            }
            catch
            {
                return null;
            }
        }
    }
}
