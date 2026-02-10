using System.Security.Cryptography;
using System.Text;
using Resonance.BusinessLogicLayer.DTOs;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;

namespace Resonance.BusinessLogicLayer.Services
{
    /// <summary>
    /// Service voor authenticatie en gebruikersbeheer
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool Success, string Message, UserDto? User)> RegisterAsync(RegisterDto dto)
        {
            // Check of email al bestaat
            if (await _unitOfWork.UserRepository.EmailExistsAsync(dto.Email))
            {
                return (false, "Dit email adres is al in gebruik", null);
            }

            // Valideer wachtwoord
            if (dto.Wachtwoord != dto.BevestigWachtwoord)
            {
                return (false, "Wachtwoorden komen niet overeen", null);
            }

            if (dto.Wachtwoord.Length < 6)
            {
                return (false, "Wachtwoord moet minimaal 6 karakters zijn", null);
            }

            // Hash wachtwoord
            var wachtwoordHash = HashPassword(dto.Wachtwoord);

            // Maak nieuwe gebruiker
            var user = new User
            {
                Email = dto.Email.ToLower(),
                WachtwoordHash = wachtwoordHash,
                UserID = 0, // Standaard UserID voor nieuwe gebruikers
                Rol = 1 // Standaard rol voor nieuwe gebruikers
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = MapToDto(user);
            return (true, "Registratie succesvol!", userDto);
        }

        public async Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto dto)
        {
            // Zoek gebruiker op email
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(dto.Email);
            
            if (user == null)
            {
                return (false, "Ongeldig email adres of wachtwoord", null);
            }

            // Verificeer wachtwoord
            if (!VerifyPassword(dto.Wachtwoord, user.WachtwoordHash ?? ""))
            {
                return (false, "Ongeldig email adres of wachtwoord", null);
            }

            var userDto = MapToDto(user);
            return (true, "Login succesvol!", userDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        // Wachtwoord hashing met SHA256
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Verificatie van wachtwoord
        private static bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }

        // Mapping
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserID = user.UserID,
                Rol = user.Rol
            };
        }
    }
}

