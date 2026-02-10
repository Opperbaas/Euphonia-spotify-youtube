using System;

namespace Resonance.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// DTO voor het ophalen van notificaties
    /// </summary>
    public class NotificatieDto
    {
        public int NotificatieID { get; set; }
        public int? UserID { get; set; }
        public string? Tekst { get; set; }
        public DateTime? DatumTijd { get; set; }
        public string? Type { get; set; } // 'Info', 'Success', 'Warning', 'Error'
        public bool IsGelezen { get; set; }
        public string? Link { get; set; }
        public string? Icoon { get; set; }
        
        // Calculated property voor tijdweergave
        public string TijdGeleden
        {
            get
            {
                if (!DatumTijd.HasValue) return "Onbekend";
                
                var verschil = DateTime.Now - DatumTijd.Value;
                
                if (verschil.TotalMinutes < 1)
                    return "Zojuist";
                if (verschil.TotalMinutes < 60)
                    return $"{(int)verschil.TotalMinutes} minuten geleden";
                if (verschil.TotalHours < 24)
                    return $"{(int)verschil.TotalHours} uur geleden";
                if (verschil.TotalDays < 7)
                    return $"{(int)verschil.TotalDays} dagen geleden";
                
                return DatumTijd.Value.ToString("d MMM yyyy");
            }
        }

        // CSS class voor badge kleur
        public string BadgeClass
        {
            get
            {
                return Type?.ToLower() switch
                {
                    "success" => "bg-success",
                    "warning" => "bg-warning",
                    "error" => "bg-danger",
                    _ => "bg-info"
                };
            }
        }
    }

    /// <summary>
    /// DTO voor het aanmaken van nieuwe notificaties
    /// </summary>
    public class CreateNotificatieDto
    {
        public int UserID { get; set; }
        public string Tekst { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public string? Link { get; set; }
        public string? Icoon { get; set; }
    }

    /// <summary>
    /// DTO voor notificatie overzicht
    /// </summary>
    public class NotificatieOverzichtDto
    {
        public int TotaalNotificaties { get; set; }
        public int OngelezenNotificaties { get; set; }
        public System.Collections.Generic.List<NotificatieDto> RecenteNotificaties { get; set; } = new();
    }
}

