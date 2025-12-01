using Euphonia.BusinessLogicLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euphonia.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Interface voor Spotify Web API integratie
    /// </summary>
    public interface ISpotifyService
    {
        /// <summary>
        /// Zoekt naar tracks op Spotify
        /// </summary>
        Task<List<ExternalMusicSearchResultDto>> SearchTracksAsync(string query, int limit = 10);

        /// <summary>
        /// Haalt een specifieke track op via Spotify ID
        /// </summary>
        Task<ExternalMusicSearchResultDto?> GetTrackByIdAsync(string trackId);
    }

    /// <summary>
    /// Interface voor YouTube Data API integratie
    /// </summary>
    public interface IYouTubeService
    {
        /// <summary>
        /// Haalt metadata op van een YouTube video
        /// </summary>
        Task<ExternalMusicSearchResultDto?> GetVideoMetadataAsync(string videoUrl);

        /// <summary>
        /// Extraheert video ID uit YouTube URL
        /// </summary>
        string? ExtractVideoId(string videoUrl);
    }
}
