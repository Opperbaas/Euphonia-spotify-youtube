using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Resonance.BusinessLogicLayer.DTOs
{
    public class StemmingDto
    {
        public int StemmingID { get; set; }
        public int? UserID { get; set; }
        public int? TypeID { get; set; }
        public DateTime? DatumTijd { get; set; }
        public string? Beschrijving { get; set; }
        
        // Extra properties voor display
        public string? TypeNaam { get; set; }
        
        // Gekoppelde muziek
        public List<MuziekDto>? GekoppeldeMuziek { get; set; }
    }

    public class CreateStemmingDto
    {
        [Required(ErrorMessage = "Type is verplicht")]
        public int TypeID { get; set; }

        [MaxLength(255, ErrorMessage = "Beschrijving mag maximaal 255 tekens zijn")]
        public string? Beschrijving { get; set; }
        
        // Muziek IDs om te koppelen
        public List<int>? MuziekIDs { get; set; }
    }

    public class UpdateStemmingDto
    {
        [Required(ErrorMessage = "Stemming ID is verplicht")]
        public int StemmingID { get; set; }

        [Required(ErrorMessage = "Type is verplicht")]
        public int TypeID { get; set; }

        [MaxLength(255, ErrorMessage = "Beschrijving mag maximaal 255 tekens zijn")]
        public string? Beschrijving { get; set; }
        
        // Muziek IDs om te koppelen
        public List<int>? MuziekIDs { get; set; }
    }

    public class LinkMuziekToStemmingDto
    {
        [Required(ErrorMessage = "Stemming ID is verplicht")]
        public int StemmingID { get; set; }
        
        [Required(ErrorMessage = "Muziek ID is verplicht")]
        public int MuziekID { get; set; }
    }
}

