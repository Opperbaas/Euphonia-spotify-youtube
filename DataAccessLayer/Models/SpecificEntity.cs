using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Euphonia.DataAccessLayer.Models
{
    /// <summary>
    /// Entity model - database tabel representatie
    /// </summary>
    [Table("SpecificEntities")]
    public class SpecificEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties (foreign keys)
        // public int RelatedEntityId { get; set; }
        // public virtual RelatedEntity RelatedEntity { get; set; }

        // Collections voor 1-to-many relaties
        // public virtual ICollection<ChildEntity> Children { get; set; }
    }
}
