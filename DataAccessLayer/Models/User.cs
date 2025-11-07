using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// User entity - gebruikersaccount voor authenticatie
    /// </summary>
    [Table("table1_user")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user")]
        public int Id { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [MaxLength(50)]
        [Column("e-mail")]
        public string? Email { get; set; }

        [MaxLength(50)]
        [Column("wachtwoord")]
        public string? WachtwoordHash { get; set; }

        [Column("rol")]
        public decimal? Rol { get; set; }

        // Extra properties voor tracking (niet in database)
        [NotMapped]
        public DateTime AangemaaktOp { get; set; } = DateTime.Now;

        [NotMapped]
        public DateTime? LaatstIngelogd { get; set; }
    }
}
