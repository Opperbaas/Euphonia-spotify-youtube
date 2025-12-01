namespace Euphonia.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// DTO voor het lezen van Profiel data
    /// </summary>
    public class ProfielDto
    {
        public int ProfielID { get; set; }
        public int? UserID { get; set; }
        public string? VoorkeurGenres { get; set; }
        public string? Stemmingstags { get; set; }
    }

    /// <summary>
    /// DTO voor het aanmaken van een nieuw Profiel
    /// </summary>
    public class CreateProfielDto
    {
        public int? UserID { get; set; }
        public string? VoorkeurGenres { get; set; }
        public string? Stemmingstags { get; set; }
    }

    /// <summary>
    /// DTO voor het updaten van een Profiel
    /// </summary>
    public class UpdateProfielDto
    {
        public int ProfielID { get; set; }
        public int? UserID { get; set; }
        public string? VoorkeurGenres { get; set; }
        public string? Stemmingstags { get; set; }
    }
}
