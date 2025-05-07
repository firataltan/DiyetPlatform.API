using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiyetPlatform.API.Models.DTOs.Auth;
using DiyetPlatform.API.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DiyetPlatform.API.Helpers
{
    public interface ITokenHelper
    {
        TokenResponseDto CreateToken(User user);
    }

    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _config;

        public TokenHelper(IConfiguration config)
        {
            _config = config;
        }

        public TokenResponseDto CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (user.Profile?.IsDietitian == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Dietitian"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpiration = DateTime.UtcNow.AddDays(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiration,
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return new TokenResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = tokenExpiration
            };
        }
    }
}