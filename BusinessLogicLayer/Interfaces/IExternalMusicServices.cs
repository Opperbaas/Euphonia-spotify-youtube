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

    
    public interface IYouTubeService
    {
        
        Task<ExternalMusicSearchResultDto?> GetVideoMetadataAsync(string videoUrl);

        /// <summary>
        /// Extraheert video ID uit YouTube URL
        /// </summary>
        string? ExtractVideoId(string videoUrl);
    }
}
