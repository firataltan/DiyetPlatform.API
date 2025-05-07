using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.User;
using DiyetPlatform.API.Models.Entities;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
{
    public interface IUserService
    {
        Task<PagedList<UserDto>> GetUsersAsync(UserParams userParams);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<ServiceResponse<UserDto>> UpdateProfileAsync(int userId, ProfileUpdateDto profileDto);
        Task<ServiceResponse<object>> FollowUserAsync(int userId, int targetId);
        Task<ServiceResponse<object>> UnfollowUserAsync(int userId, int targetId);
        Task<PagedList<UserDto>> GetUserFollowersAsync(int userId, UserParams userParams);
        Task<PagedList<UserDto>> GetUserFollowingAsync(int userId, UserParams userParams);
        Task<PagedList<UserDto>> SearchUsersAsync(string searchTerm, UserParams userParams);
        Task CreateUserAsync(string email, string firstName, string lastName);
    }
}
