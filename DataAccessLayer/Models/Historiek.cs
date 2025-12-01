using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// Historiek entity - gebruiksgeschiedenis met periode
    /// </summary>
    [Table("historiek")]
    public class Historiek
    {
        [Key]
        [Column("historiekID")]
        public int HistoriekID { get; set; }

        [Column("userID")]
        public int? UserID { get; set; }

        [Column("periode_start")]
        public DateTime? PeriodeStart { get; set; }

        [Column("periode_einde")]
        public DateTime? PeriodeEinde { get; set; }
    }
}
