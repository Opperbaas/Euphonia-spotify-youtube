using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resonance.DataAccessLayer.Models
{
    /// <summary>
    /// Statistiek entity - gebruikersstatistieken en trends
    /// </summary>
    [Table("statistiek")]
    public class Statistiek
    {
        [Key]
        [Column("statistiekID")]
        public int StatistiekID { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [MaxLength(255)]
        [Column("trend_type")]
        public string? TrendType { get; set; }

        [MaxLength(255)]
        [Column("resultaat")]
        public string? Resultaat { get; set; }
    }
}

