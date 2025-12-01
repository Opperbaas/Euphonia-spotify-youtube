using System.ComponentModel.DataAnnotations;

namespace Euphonia.BusinessLogicLayer.DTOs
{
    /// <summary>
    /// DTO voor gebruikersregistratie
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig email adres")]
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [MinLength(6, ErrorMessage = "Wachtwoord moet minimaal 6 karakters zijn")]
        [MaxLength(50)]
        public string Wachtwoord { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bevestig wachtwoord is verplicht")]
        [Compare("Wachtwoord", ErrorMessage = "Wachtwoorden komen niet overeen")]
        public string BevestigWachtwoord { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO voor gebruikerslogin
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig email adres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        public string Wachtwoord { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO voor ingelogde gebruiker
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int? UserID { get; set; }
        public decimal? Rol { get; set; }
    }
}
