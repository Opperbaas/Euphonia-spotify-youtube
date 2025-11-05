using System;

namespace Euphonia.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// Data Transfer Object - gebruikt voor data overdracht tussen lagen
    /// Bevat alleen de data die nodig is voor de presentatielaag
    /// </summary>
    public class SpecificDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Voeg alleen properties toe die de presentatielaag nodig heeft
        // Verberg complexe relaties of gevoelige data
    }

    /// <summary>
    /// Create DTO - gebruikt voor het aanmaken van nieuwe entities
    /// </summary>
    public class CreateSpecificDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        // Geen Id - wordt door database gegenereerd
    }

    /// <summary>
    /// Update DTO - gebruikt voor het updaten van entities
    /// </summary>
    public class UpdateSpecificDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
