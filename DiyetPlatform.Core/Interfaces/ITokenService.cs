using DiyetPlatform.Core.Entities;

namespace DiyetPlatform.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        bool ValidateToken(string token);
    }
} 