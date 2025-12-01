using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// Stemming entity - gebruikersstemming op een moment
    /// </summary>
    [Table("stemming")]
    public class Stemming
    {
        [Key]
        [Column("stemmingID")]
        public int StemmingID { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [Column("typeID")]
        public int? TypeID { get; set; }

        [Column("datum_tijd")]
        public DateTime? DatumTijd { get; set; }

        [MaxLength(255)]
        [Column("beschrijving")]
        public string? Beschrijving { get; set; }

        // Navigation properties
        [ForeignKey("TypeID")]
        public virtual StemmingType? StemmingType { get; set; }
        
        public virtual ICollection<StemmingMuziek> StemmingMuzieks { get; set; } = new List<StemmingMuziek>();
    }
}
