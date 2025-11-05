using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
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

        [MaxLength(255)]
        [Column("label")]
        public string? Label { get; set; }

        [MaxLength(255)]
        [Column("emoji")]
        public string? Emoji { get; set; }

        [Column("waarde")]
        public int? Waarde { get; set; }

        // Navigation property
        public virtual ICollection<Stemming> Stemmingen { get; set; } = new List<Stemming>();
    }
}
