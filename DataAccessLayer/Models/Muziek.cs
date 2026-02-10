using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resonance.DataAccessLayer.Models
{
    /// <summary>
    /// Muziek entity - representeert een muzieknummer
    /// </summary>
    [Table("muziek")]
    public class Muziek
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("muziekID")]
        public int MuziekID { get; set; }

        [MaxLength(255)]
        [Column("titel")]
        public string? Titel { get; set; }

        [MaxLength(255)]
        [Column("artiest")]
        public string? Artiest { get; set; }

        [MaxLength(255)]
        [Column("bron")]
        public string? Bron { get; set; }

        [MaxLength(50)]
        [Column("youtubeVideoId")]
        public string? YouTubeVideoId { get; set; }

        [MaxLength(512)]
        [Column("youtubeThumbnailUrl")]
        public string? YouTubeThumbnailUrl { get; set; }

        [MaxLength(50)]
        [Column("spotifyTrackId")]
        public string? SpotifyTrackId { get; set; }

        // Navigation properties
        public virtual ICollection<MuziekAnalyse> Analyses { get; set; } = new List<MuziekAnalyse>();
        public virtual ICollection<StemmingMuziek> StemmingMuzieks { get; set; } = new List<StemmingMuziek>();
    }
}

