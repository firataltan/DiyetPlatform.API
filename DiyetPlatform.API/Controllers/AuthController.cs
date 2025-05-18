using DiyetPlatform.Application.DTOs.Auth;
using DiyetPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiyetPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var userId = int.Parse(User.FindFirst("nameidentifier")?.Value);
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(forgotPasswordDto.Email);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}