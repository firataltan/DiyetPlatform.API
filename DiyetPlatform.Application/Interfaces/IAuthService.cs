using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.Auth;

namespace DiyetPlatform.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> RegisterAsync(RegisterDto registerDto);
        Task<ServiceResponse<string>> LoginAsync(string email, string password);
        Task<ServiceResponse<object>> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<ServiceResponse<object>> ResetPasswordAsync(string email);
        Task<ServiceResponse<object>> VerifyEmailAsync(string token);
        Task<ServiceResponse<object>> RefreshTokenAsync(string refreshToken);
    }
} 