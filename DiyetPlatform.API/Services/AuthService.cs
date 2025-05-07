using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.Auth;
using DiyetPlatform.API.Models.Entities;
using System.Threading.Tasks;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<TokenResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ServiceResponse<TokenResponseDto>> LoginAsync(LoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenHelper _tokenHelper;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenHelper tokenHelper,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenHelper = tokenHelper;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResponse<TokenResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            var response = new ServiceResponse<TokenResponseDto>();

            // Kullanıcı adı veya e-posta zaten varsa hata döndür
            if (await _unitOfWork.UserRepository.IsUsernameExistsAsync(registerDto.Username))
            {
                response.Success = false;
                response.Message = "Bu kullanıcı adı zaten kullanılıyor.";
                return response;
            }

            if (await _unitOfWork.UserRepository.IsEmailExistsAsync(registerDto.Email))
            {
                response.Success = false;
                response.Message = "Bu e-posta adresi zaten kullanılıyor.";
                return response;
            }

            // Şifreyi hashle
            _passwordHasher.CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Yeni kullanıcı oluştur
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Profile = new Profile
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    IsDietitian = registerDto.IsDietitian,
                    Bio = "",
                    Location = "",
                    Website = "",
                    ProfileImageUrl = "",
                    Specialization = "",
                    Certification = "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Kullanıcıyı veritabanına ekle
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.Complete();

            // Token oluştur
            var tokenResponse = _tokenHelper.CreateToken(user);

            response.Data = new TokenResponseDto
            {
                Token = tokenResponse.Token,
                Username = user.Username,
                UserId = user.Id,
                Expiration = tokenResponse.Expiration
            };
            response.Message = "Kayıt başarılı.";

            return response;
        }

        public async Task<ServiceResponse<TokenResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var response = new ServiceResponse<TokenResponseDto>();

            // Kullanıcıyı bul
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(loginDto.Username);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Kullanıcı adı veya şifre hatalı.";
                return response;
            }

            // Şifreyi doğrula
            if (!_passwordHasher.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Kullanıcı adı veya şifre hatalı.";
                return response;
            }

            // Son giriş zamanını güncelle
            user.LastActive = DateTime.UtcNow;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.Complete();

            // Token oluştur
            var tokenResponse = _tokenHelper.CreateToken(user);

            response.Data = new TokenResponseDto
            {
                Token = tokenResponse.Token,
                Username = user.Username,
                UserId = user.Id,
                Expiration = tokenResponse.Expiration
            };
            response.Message = "Giriş başarılı.";

            return response;
        }
    }
}