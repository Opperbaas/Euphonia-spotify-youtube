using Euphonia.BusinessLogicLayer.DTOs;
using Euphonia.BusinessLogicLayer.Interfaces;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euphonia.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service voor Spotify Web API integratie
    /// </summary>
    public class SpotifyService : ISpotifyService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private SpotifyClient? _spotifyClient;

        public SpotifyService(IConfiguration configuration)
        {
            _clientId = configuration["ExternalMusicApis:Spotify:ClientId"] ?? "";
            _clientSecret = configuration["ExternalMusicApis:Spotify:ClientSecret"] ?? "";
        }

        /// <summary>
        /// Initialiseert Spotify client met authenticatie
        /// </summary>
        private async Task<SpotifyClient> GetAuthenticatedClientAsync()
        {
            if (_spotifyClient != null)
                return _spotifyClient;

            // Controleer of credentials zijn ingesteld
            if (string.IsNullOrEmpty(_clientId) || _clientId == "YOUR_SPOTIFY_CLIENT_ID")
            {
                throw new InvalidOperationException(
                    "Spotify API credentials zijn niet ingesteld in appsettings.json. " +
                    "Bezoek https://developer.spotify.com/dashboard voor het aanmaken van een app."
                );
            }

            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(_clientId, _clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            _spotifyClient = new SpotifyClient(config.WithToken(response.AccessToken));
            return _spotifyClient;
        }

        /// <summary>
        /// Zoekt naar tracks op Spotify
        /// </summary>
        public async Task<List<ExternalMusicSearchResultDto>> SearchTracksAsync(string query, int limit = 10)
        {
            try
            {
                var spotify = await GetAuthenticatedClientAsync();
                var searchRequest = new SearchRequest(SearchRequest.Types.Track, query)
                {
                    Limit = limit
                };

                var searchResponse = await spotify.Search.Item(searchRequest);
                
                if (searchResponse.Tracks?.Items == null)
                    return new List<ExternalMusicSearchResultDto>();

                return searchResponse.Tracks.Items.Select(track => new ExternalMusicSearchResultDto
                {
                    ExternalId = track.Id,
                    Titel = track.Name,
                    Artiest = string.Join(", ", track.Artists.Select(a => a.Name)),
                    Platform = "Spotify",
                    AlbumImageUrl = track.Album?.Images?.FirstOrDefault()?.Url,
                    PreviewUrl = track.PreviewUrl,
                    ExternalUrl = track.ExternalUrls?.FirstOrDefault().Value,
                    DurationMs = track.DurationMs
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log error (in productie zou je hier proper logging doen)
                Console.WriteLine($"Spotify API Error: {ex.Message}");
                return new List<ExternalMusicSearchResultDto>();
            }
        }

        /// <summary>
        /// Haalt een specifieke track op via Spotify ID
        /// </summary>
        public async Task<ExternalMusicSearchResultDto?> GetTrackByIdAsync(string trackId)
        {
            try
            {
                var spotify = await GetAuthenticatedClientAsync();
                var track = await spotify.Tracks.Get(trackId);

                if (track == null)
                    return null;

                return new ExternalMusicSearchResultDto
                {
                    ExternalId = track.Id,
                    Titel = track.Name,
                    Artiest = string.Join(", ", track.Artists.Select(a => a.Name)),
                    Platform = "Spotify",
                    AlbumImageUrl = track.Album?.Images?.FirstOrDefault()?.Url,
                    PreviewUrl = track.PreviewUrl,
                    ExternalUrl = track.ExternalUrls?.FirstOrDefault().Value,
                    DurationMs = track.DurationMs
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Spotify API Error: {ex.Message}");
                return null;
            }
        }
    }
}
