using System;
using System.Collections.Generic;

namespace Euphonia.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// DTO voor Dashboard statistieken en inzichten
    /// </summary>
    public class DashboardDto
    {
        // Algemene statistieken
        public int TotaalStemmingen { get; set; }
        public int TotaalMuziek { get; set; }
        public int TotaalGekoppeldeMuziek { get; set; }
        public DateTime? LaatsteStemmingDatum { get; set; }
        
        // Stemming statistieken
        public List<StemmingTypeStatistiekDto> StemmingTypeStatistieken { get; set; } = new();
        public StemmingTypeDto? MeestVoorkomendeeStemming { get; set; }
        public List<StemmingTrendDto> StemmingTrendPerDag { get; set; } = new();
        
        // Muziek statistieken
        public List<ArtiestStatistiekDto> TopArtiesten { get; set; } = new();
        public List<MuziekStemmingCorrelatie> MuziekStemmingCorrelaties { get; set; } = new();
        public List<MuziekDto> MeestGekoppeldeMuziek { get; set; } = new();
        
        // Inzichten
        public List<string> Inzichten { get; set; } = new();
    }

    public class StemmingTypeStatistiekDto
    {
        public int TypeID { get; set; }
        public string? TypeNaam { get; set; }
        public int Aantal { get; set; }
        public double Percentage { get; set; }
    }

    public class StemmingTrendDto
    {
        public DayOfWeek DagVanWeek { get; set; }
        public string? MeestVoorkomendeeStemming { get; set; }
        public int Aantal { get; set; }
    }

    public class ArtiestStatistiekDto
    {
        public string? Artiest { get; set; }
        public int AantalLiedjes { get; set; }
        public int AantalKoppelingen { get; set; }
        public List<string> MeestBijStemmingen { get; set; } = new();
    }

    public class MuziekStemmingCorrelatie
    {
        public string? MuziekTitel { get; set; }
        public string? Artiest { get; set; }
        public string? StemmingType { get; set; }
        public int AantalKeerGekoppeld { get; set; }
    }
}
