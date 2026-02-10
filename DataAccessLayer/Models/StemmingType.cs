using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resonance.DataAccessLayer.Models
{
    /// <summary>
    /// StemmingType entity - types van stemmingen (bv. blij, verdrietig, etc.)
    /// </summary>
    [Table("stemmingType")]
    public class StemmingType
    {
        [Key]
        [Column("typeID")]
        public int TypeID { get; set; }

        [MaxLength(100)]
        [Column("naam")]
        public string? Naam { get; set; }

        [MaxLength(500)]
        [Column("beschrijving")]
        public string? Beschrijving { get; set; }

        // Navigation property
        public virtual ICollection<Stemming> Stemmingen { get; set; } = new List<Stemming>();
    }
}

