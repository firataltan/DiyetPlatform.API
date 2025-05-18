using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.User;

namespace DiyetPlatform.Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<UserDto>> GetUsersAsync(Application.Common.Parameters.UserParams userParams);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<ServiceResponse<UserDto>> UpdateUserAsync(int userId, ProfileUpdateDto profileDto);
        Task<ServiceResponse<object>> DeleteUserAsync(int userId);
        Task<ServiceResponse<object>> FollowUserAsync(int userId, int targetUserId);
        Task<ServiceResponse<object>> UnfollowUserAsync(int userId, int targetUserId);
        Task<PagedList<UserDto>> GetFollowersAsync(int userId, Application.Common.Parameters.UserParams userParams);
        Task<PagedList<UserDto>> GetFollowingAsync(int userId, Application.Common.Parameters.UserParams userParams);
    }
}
