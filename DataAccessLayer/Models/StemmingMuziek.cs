using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// StemmingMuziek entity - koppeltabel tussen Stemming en Muziek
    /// </summary>
    [Table("stemmingMuziek")]
    public class StemmingMuziek
    {
        [Key]
        [Column("PK")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PK { get; set; }

        [Column("stemmingID")]
        public int? StemmingID { get; set; }

        [Column("muziekID")]
        public int? MuziekID { get; set; }

        // Navigation properties
        [ForeignKey("StemmingID")]
        public virtual Stemming? Stemming { get; set; }

        [ForeignKey("MuziekID")]
        public virtual Muziek? Muziek { get; set; }
    }
}
