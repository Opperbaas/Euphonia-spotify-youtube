using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Resonance.DataAccessLayer.Models
{
    /// <summary>
    /// Notificatie entity - gebruikersnotificaties
    /// </summary>
    [Table("notificatie")]
    public class Notificatie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("notificatieID")]
        public int NotificatieID { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [MaxLength(255)]
        [Column("tekst")]
        public string? Tekst { get; set; }

        [Column("datum_tijd")]
        public DateTime? DatumTijd { get; set; }

        [MaxLength(50)]
        [Column("type")]
        public string? Type { get; set; } // 'Info', 'Success', 'Warning', 'Error'

        [Column("isGelezen")]
        public bool IsGelezen { get; set; }

        [MaxLength(500)]
        [Column("link")]
        public string? Link { get; set; }

        [MaxLength(50)]
        [Column("icoon")]
        public string? Icoon { get; set; } // Bootstrap icon class

        // Navigation property
        public virtual User? User { get; set; }
    }
}

