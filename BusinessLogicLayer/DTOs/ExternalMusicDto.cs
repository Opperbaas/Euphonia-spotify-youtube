namespace Resonance.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// DTO voor externe muziek zoekresultaten (Spotify/YouTube)
    /// </summary>
    public class ExternalMusicSearchResultDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Titel { get; set; } = string.Empty;
        public string Artiest { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty; // "Spotify" of "YouTube"
        public string? AlbumImageUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public string? ExternalUrl { get; set; }
        public int? DurationMs { get; set; }
    }

    /// <summary>
    /// DTO voor Spotify zoekparameters
    /// </summary>
    public class SpotifySearchDto
    {
        public string Query { get; set; } = string.Empty;
        public int Limit { get; set; } = 10;
    }

    /// <summary>
    /// DTO voor YouTube video metadata
    /// </summary>
    public class YouTubeVideoDto
    {
        public string VideoId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}

