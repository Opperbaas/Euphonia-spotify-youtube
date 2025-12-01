using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// Profiel entity - gebruikersprofiel met voorkeuren
    /// </summary>
    [Table("profiel")]
    public class Profiel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("profielID")]
        public int ProfielID { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [MaxLength(255)]
        [Column("voorkeur_genres")]
        public string? VoorkeurGenres { get; set; }

        [MaxLength(255)]
        [Column("stemmingstags")]
        public string? Stemmingstags { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; } = false;
    }
}
