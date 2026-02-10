using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resonance.DataAccessLayer.Models
{
    /// <summary>
    /// MuziekAnalyse entity - analyse gegevens van muziek
    /// </summary>
    [Table("muziekAnalyse")]
    public class MuziekAnalyse
    {
        [Key]
        [Column("analyseID")]
        public int AnalyseID { get; set; }

        [Column("muziekID")]
        public int? MuziekID { get; set; }

        [MaxLength(255)]
        [Column("stemmingType")]
        public string? StemmingType { get; set; }

        [MaxLength(255)]
        [Column("energieLevel")]
        public string? EnergieLevel { get; set; }

        [Column("valence")]
        public int? Valence { get; set; }

        [MaxLength(255)]
        [Column("tempo")]
        public string? Tempo { get; set; }

        [MaxLength(255)]
        [Column("bron")]
        public string? Bron { get; set; }

        // Navigation property
        [ForeignKey("MuziekID")]
        public virtual Muziek? Muziek { get; set; }
    }
}

