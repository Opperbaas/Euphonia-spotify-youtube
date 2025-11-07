using Euphonia.BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Euphonia.PresentationLayer.Controllers
{
    /// <summary>
    /// API Controller voor externe muziek bronnen (Spotify & YouTube)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalMusicController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        private readonly IYouTubeService _youtubeService;

        public ExternalMusicController(ISpotifyService spotifyService, IYouTubeService youtubeService)
        {
            _spotifyService = spotifyService;
            _youtubeService = youtubeService;
        }

        /// <summary>
        /// Zoekt tracks op Spotify
        /// GET: api/ExternalMusic/spotify/search?query=bohemian+rhapsody
        /// </summary>
        [HttpGet("spotify/search")]
        public async Task<IActionResult> SearchSpotify([FromQuery] string query, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query parameter is verplicht" });

            try
            {
                var results = await _spotifyService.SearchTracksAsync(query, limit);
                return Ok(results);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Er is een fout opgetreden bij het zoeken op Spotify", details = ex.Message });
            }
        }

        /// <summary>
        /// Haalt een Spotify track op via ID
        /// GET: api/ExternalMusic/spotify/track/3n3Ppam7vgaVa1iaRUc9Lp
        /// </summary>
        [HttpGet("spotify/track/{trackId}")]
        public async Task<IActionResult> GetSpotifyTrack(string trackId)
        {
            if (string.IsNullOrWhiteSpace(trackId))
                return BadRequest(new { error = "Track ID is verplicht" });

            try
            {
                var result = await _spotifyService.GetTrackByIdAsync(trackId);
                if (result == null)
                    return NotFound(new { error = "Track niet gevonden" });

                return Ok(result);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Er is een fout opgetreden", details = ex.Message });
            }
        }

        /// <summary>
        /// Haalt YouTube video metadata op
        /// GET: api/ExternalMusic/youtube/video?url=https://www.youtube.com/watch?v=dQw4w9WgXcQ
        /// </summary>
        [HttpGet("youtube/video")]
        public async Task<IActionResult> GetYouTubeVideo([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(new { error = "URL parameter is verplicht" });

            try
            {
                var result = await _youtubeService.GetVideoMetadataAsync(url);
                if (result == null)
                    return NotFound(new { error = "Video niet gevonden of ongeldige URL" });

                return Ok(result);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Er is een fout opgetreden", details = ex.Message });
            }
        }
    }
}
