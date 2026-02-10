using System.ComponentModel.DataAnnotations;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class StemmingTypeDto
    {
        public int TypeID { get; set; }
        
        [Required(ErrorMessage = "Naam is verplicht")]
        [MaxLength(100, ErrorMessage = "Naam mag maximaal 100 tekens zijn")]
        public string? Naam { get; set; }
        
        [MaxLength(500, ErrorMessage = "Beschrijving mag maximaal 500 tekens zijn")]
        public string? Beschrijving { get; set; }
    }
}

