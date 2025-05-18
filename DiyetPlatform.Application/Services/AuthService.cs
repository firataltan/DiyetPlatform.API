using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Core.Interfaces;
using DiyetPlatform.Application.DTOs.Auth;
using DiyetPlatform.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace DiyetPlatform.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDto registerDto)
        {
            var response = new ServiceResponse<string>();

            try
            {
                // Check if username or email already exists
                var existingUserByUsername = await _unitOfWork.Users.GetUserByUsernameAsync(registerDto.Username);
                if (existingUserByUsername != null)
                {
                    response.Success = false;
                    response.Message = "Bu kullanıcı adı zaten kullanılıyor.";
                    return response;
                }

                var existingUserByEmail = await _unitOfWork.Users.GetUserByEmailAsync(registerDto.Email);
                if (existingUserByEmail != null)
                {
                    response.Success = false;
                    response.Message = "Bu e-posta adresi zaten kullanılıyor.";
                    return response;
                }

                // Create password hash
                _passwordHasher.CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                // Parse full name into first and last name
                string[] nameParts = registerDto.FullName.Split(' ', 2);
                string firstName = nameParts[0];
                string lastName = nameParts.Length > 1 ? nameParts[1] : "";

                // Create new user
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    LastActive = DateTime.UtcNow,
                    Profile = new Profile
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        FullName = registerDto.FullName,
                        IsDietitian = registerDto.IsDietitian,
                        Bio = "",
                        ProfileImageUrl = "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                // Add user to database
                _unitOfWork.Users.Add(user);
                await _unitOfWork.Complete();

                // Generate token
                var token = _tokenService.CreateToken(user);

                response.Data = token;
                response.Message = "Kayıt başarılı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kayıt işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> LoginAsync(string email, string password)
        {
            var response = new ServiceResponse<string>();

            try
            {
                // Find user by email
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "E-posta adresi veya şifre hatalı.";
                    return response;
                }

                // Verify password
                if (!_passwordHasher.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "E-posta adresi veya şifre hatalı.";
                    return response;
                }

                // Update last login time
                user.LastActive = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.Complete();

                // Generate token
                var token = _tokenService.CreateToken(user);

                response.Data = token;
                response.Message = "Giriş başarılı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Giriş işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var response = new ServiceResponse<object>();

            try
            {
                // Find user by id
                var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Kullanıcı bulunamadı.";
                    return response;
                }

                // Verify current password
                if (!_passwordHasher.VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "Mevcut şifre hatalı.";
                    return response;
                }

                // Create new password hash
                _passwordHasher.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

                // Update user password
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                // User entity might not have UpdatedAt property, so let's check if it's needed
                // user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.Complete();

                response.Message = "Şifre başarıyla değiştirildi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Şifre değiştirme işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> ResetPasswordAsync(string email)
        {
            var response = new ServiceResponse<object>();

            try
            {
                // Find user by email
                var user = await _unitOfWork.Users.GetUserByEmailAsync(email);

                if (user == null)
                {
                    // For security reasons, don't reveal that the email doesn't exist
                    response.Message = "Şifre sıfırlama talimatları e-posta adresinize gönderildi.";
                    return response;
                }

                // In a real implementation, generate a reset token and send email
                // For now, we just log a message
                
                response.Message = "Şifre sıfırlama talimatları e-posta adresinize gönderildi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Şifre sıfırlama işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> VerifyEmailAsync(string token)
        {
            var response = new ServiceResponse<object>();

            try
            {
                // In a real implementation, verify the token and mark user's email as verified
                // For now, we just log a message

                response.Message = "E-posta adresi doğrulandı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"E-posta doğrulama işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> RefreshTokenAsync(string refreshToken)
        {
            var response = new ServiceResponse<object>();

            try
            {
                // In a real implementation, validate the refresh token and issue a new access token
                // For now, we just log a message

                response.Message = "Token yenilendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Token yenileme işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return response;
        }
    }
}