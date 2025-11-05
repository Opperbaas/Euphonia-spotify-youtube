using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// Notificatie entity - gebruikersnotificaties
    /// </summary>
    [Table("notificatie")]
    public class Notificatie
    {
        [Key]
        [Column("notificatieID")]
        public int NotificatieID { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [MaxLength(255)]
        [Column("tekst")]
        public string? Tekst { get; set; }

        [Column("datum_tijd")]
        public DateTime? DatumTijd { get; set; }
    }
}
