using AutoMapper;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.User;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiyetPlatform.Infrastructure.Repositories;

namespace DiyetPlatform.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment,
            INotificationService notificationService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<PagedList<UserDto>> GetUsersAsync(Application.Common.Parameters.UserParams userParams)
        {
            var query = _unitOfWork.Users.GetUsersQuery();

            // Apply filters
            if (!string.IsNullOrEmpty(userParams.Search))
            {
                query = query.Where(u => 
                    u.Username.ToLower().Contains(userParams.Search.ToLower()) ||
                    u.Profile.FirstName.ToLower().Contains(userParams.Search.ToLower()) ||
                    u.Profile.LastName.ToLower().Contains(userParams.Search.ToLower()) ||
                    u.Profile.FullName.ToLower().Contains(userParams.Search.ToLower()));
            }

            if (userParams.IsDietitian)
            {
                query = query.Where(u => u.Profile.IsDietitian == userParams.IsDietitian);
            }

            // Apply sorting
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "lastActive" => query.OrderByDescending(u => u.LastActive),
                "followers" => query.OrderByDescending(u => u.Followers.Count),
                _ => query.OrderBy(u => u.Username)
            };

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((userParams.PageNumber - 1) * userParams.PageSize)
                .Take(userParams.PageSize)
                .ToListAsync();

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new PagedList<UserDto>(
                userDtos,
                totalCount,
                userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(id);

            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<ServiceResponse<UserDto>> UpdateUserAsync(int userId, ProfileUpdateDto profileDto)
        {
            var response = new ServiceResponse<UserDto>();

            try
            {
                var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Kullanıcı bulunamadı.";
                    return response;
                }

                // Update profile information
                _mapper.Map(profileDto, user.Profile);
                user.Profile.UpdatedAt = DateTime.UtcNow;

                // Handle profile picture upload
                if (profileDto.ProfileImage != null)
                {
                    var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "profile-images");

                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    var fileExtension = Path.GetExtension(profileDto.ProfileImage.FileName);
                    var fileName = $"{userId}_{DateTime.Now.Ticks}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profileDto.ProfileImage.CopyToAsync(fileStream);
                    }

                    // Delete old profile picture if exists
                    if (!string.IsNullOrEmpty(user.Profile.ProfileImageUrl))
                    {
                        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, user.Profile.ProfileImageUrl.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    user.Profile.ProfileImageUrl = $"/uploads/profile-images/{fileName}";
                }

                _unitOfWork.Users.Update(user);
                await _unitOfWork.Complete();

                response.Data = _mapper.Map<UserDto>(user);
                response.Message = "Profil başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Profil güncellenirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Error updating user profile");
            }

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteUserAsync(int userId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Kullanıcı bulunamadı.";
                    return response;
                }

                // Delete profile picture if exists
                if (!string.IsNullOrEmpty(user.Profile.ProfileImageUrl))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, user.Profile.ProfileImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                _unitOfWork.Users.Remove(user);
                await _unitOfWork.Complete();

                response.Message = "Kullanıcı başarıyla silindi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kullanıcı silinirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Error deleting user");
            }

            return response;
        }

        public async Task<ServiceResponse<object>> FollowUserAsync(int userId, int targetUserId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                if (userId == targetUserId)
                {
                    response.Success = false;
                    response.Message = "Kendinizi takip edemezsiniz.";
                    return response;
                }

                var targetUser = await _unitOfWork.Users.GetUserByIdAsync(targetUserId);

                if (targetUser == null)
                {
                    response.Success = false;
                    response.Message = "Takip etmek istediğiniz kullanıcı bulunamadı.";
                    return response;
                }

                var isFollowing = await _unitOfWork.Follows.IsUserFollowingAsync(userId, targetUserId);

                if (isFollowing)
                {
                    response.Success = false;
                    response.Message = "Bu kullanıcıyı zaten takip ediyorsunuz.";
                    return response;
                }

                var follow = new Follow
                {
                    FollowerId = userId,
                    FollowedId = targetUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Follows.Add(follow);
                await _unitOfWork.Complete();

                // Create notification
                await _notificationService.CreateNotificationAsync(
                    targetUserId,
                    userId,
                    $"{userId} sizi takip etmeye başladı.",
                    "follow");

                response.Message = "Kullanıcı başarıyla takip edildi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kullanıcı takip edilirken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Error following user");
            }

            return response;
        }

        public async Task<ServiceResponse<object>> UnfollowUserAsync(int userId, int targetUserId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                if (userId == targetUserId)
                {
                    response.Success = false;
                    response.Message = "Kendinizi takipten çıkaramazsınız.";
                    return response;
                }

                var follow = await _unitOfWork.Follows.GetFollowAsync(userId, targetUserId);

                if (follow == null)
                {
                    response.Success = false;
                    response.Message = "Bu kullanıcıyı zaten takip etmiyorsunuz.";
                    return response;
                }

                _unitOfWork.Follows.Remove(follow);
                await _unitOfWork.Complete();

                response.Message = "Kullanıcı takipten çıkarıldı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kullanıcı takipten çıkarılırken bir hata oluştu: {ex.Message}";
                _logger.LogError(ex, "Error unfollowing user");
            }

            return response;
        }

        public async Task<PagedList<UserDto>> GetFollowersAsync(int userId, Application.Common.Parameters.UserParams userParams)
        {
            // Get followers through the Follow entity
            var followersQuery = _unitOfWork.Follows.GetFollowsQuery()
                .Where(f => f.FollowedId == userId)
                .Select(f => f.Follower);

            // Apply sorting
            var query = userParams.OrderBy switch
            {
                "created" => followersQuery.OrderByDescending(u => u.CreatedAt),
                "lastActive" => followersQuery.OrderByDescending(u => u.LastActive),
                _ => followersQuery.OrderBy(u => u.Username)
            };

            var totalCount = await query.CountAsync();
            var followers = await query
                .Skip((userParams.PageNumber - 1) * userParams.PageSize)
                .Take(userParams.PageSize)
                .ToListAsync();

            var followerDtos = _mapper.Map<List<UserDto>>(followers);

            return new PagedList<UserDto>(
                followerDtos,
                totalCount,
                userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<PagedList<UserDto>> GetFollowingAsync(int userId, Application.Common.Parameters.UserParams userParams)
        {
            // Get following users through the Follow entity
            var followingQuery = _unitOfWork.Follows.GetFollowsQuery()
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followed);

            // Apply sorting
            var query = userParams.OrderBy switch
            {
                "created" => followingQuery.OrderByDescending(u => u.CreatedAt),
                "lastActive" => followingQuery.OrderByDescending(u => u.LastActive),
                _ => followingQuery.OrderBy(u => u.Username)
            };

            var totalCount = await query.CountAsync();
            var following = await query
                .Skip((userParams.PageNumber - 1) * userParams.PageSize)
                .Take(userParams.PageSize)
                .ToListAsync();

            var followingDtos = _mapper.Map<List<UserDto>>(following);

            return new PagedList<UserDto>(
                followingDtos,
                totalCount,
                userParams.PageNumber,
                userParams.PageSize);
        }
    }
}
