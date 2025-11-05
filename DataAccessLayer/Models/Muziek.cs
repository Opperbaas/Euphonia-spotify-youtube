using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// Muziek entity - representeert een muzieknummer
    /// </summary>
    [Table("muziek")]
    public class Muziek
    {
        [Key]
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

        // Navigation properties
        public virtual ICollection<MuziekAnalyse> Analyses { get; set; } = new List<MuziekAnalyse>();
        public virtual ICollection<StemmingMuziek> StemmingMuzieks { get; set; } = new List<StemmingMuziek>();
    }
}
