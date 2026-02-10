using Resonance.BusinessLogicLayer.DTOs;

namespace Resonance.BusinessLogicLayer.Interfaces
{
    /// <summary>
    /// Service interface voor authenticatie
    /// </summary>
    public interface IAuthService
    {
        Task<(bool Success, string Message, UserDto? User)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto dto);
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}

