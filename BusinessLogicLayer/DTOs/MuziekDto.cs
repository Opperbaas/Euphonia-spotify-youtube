using System.Collections.Generic;

namespace Euphonia.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// DTO voor het ophalen van Muziek data
    /// </summary>
    public class MuziekDto
    {
        public int MuziekID { get; set; }
        public string? Titel { get; set; }
        public string? Artiest { get; set; }
        public string? Bron { get; set; }
        public string? YouTubeVideoId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? SpotifyTrackId { get; set; }
        
        // Optioneel: analyses meegeven
        public List<MuziekAnalyseDto>? Analyses { get; set; }
    }

    /// <summary>
    /// DTO voor het aanmaken van nieuwe muziek
    /// </summary>
    public class CreateMuziekDto
    {
        public string Titel { get; set; } = string.Empty;
        public string Artiest { get; set; } = string.Empty;
        public string? Bron { get; set; }
        public string? YouTubeVideoId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? SpotifyTrackId { get; set; }
    }

    /// <summary>
    /// DTO voor het updaten van muziek
    /// </summary>
    public class UpdateMuziekDto
    {
        public int MuziekID { get; set; }
        public string Titel { get; set; } = string.Empty;
        public string Artiest { get; set; } = string.Empty;
        public string? Bron { get; set; }
    }

    /// <summary>
    /// DTO voor MuziekAnalyse (gebruikt in MuziekDto)
    /// </summary>
    public class MuziekAnalyseDto
    {
        public int AnalyseID { get; set; }
        public string? StemmingType { get; set; }
        public string? EnergieLevel { get; set; }
        public int? Valence { get; set; }
        public string? Tempo { get; set; }
    }
}
