using AutoMapper;
using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.User;
using DiyetPlatform.API.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using DiyetPlatform.API.Models;
using DiyetPlatform.API.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DiyetPlatform.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment,
            IConfiguration config,
            INotificationService notificationService,
            IPublishEndpoint publishEndpoint,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _config = config;
            _notificationService = notificationService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<PagedList<UserDto>> GetUsersAsync(UserParams userParams)
        {
            var users = await _unitOfWork.UserRepository.GetUsersAsync(userParams);
            var userDtos = _mapper.Map<PagedList<UserDto>>(users);

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            if (user == null)
                return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<ServiceResponse<UserDto>> UpdateProfileAsync(int userId, ProfileUpdateDto profileDto)
        {
            var response = new ServiceResponse<UserDto>();

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Kullanıcı bulunamadı.";
                return response;
            }

            // Profil bilgilerini güncelle
            user.Profile.FirstName = profileDto.FirstName;
            user.Profile.LastName = profileDto.LastName;
            user.Profile.Bio = profileDto.Bio;
            user.Profile.Location = profileDto.Location;
            user.Profile.Website = profileDto.Website;

            // Profil fotoğrafı yükleme
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

                // Eski profil fotoğrafını sil (eğer varsa)
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

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.Complete();

            response.Data = _mapper.Map<UserDto>(user);
            response.Message = "Profil başarıyla güncellendi.";

            return response;
        }

        public async Task<ServiceResponse<object>> FollowUserAsync(int userId, int targetId)
        {
            var response = new ServiceResponse<object>();

            if (userId == targetId)
            {
                response.Success = false;
                response.Message = "Kendinizi takip edemezsiniz.";
                return response;
            }

            var targetUser = await _unitOfWork.UserRepository.GetUserByIdAsync(targetId);

            if (targetUser == null)
            {
                response.Success = false;
                response.Message = "Takip etmek istediğiniz kullanıcı bulunamadı.";
                return response;
            }

            var isFollowing = await _unitOfWork.UserRepository.IsUserFollowingAsync(userId, targetId);

            if (isFollowing)
            {
                response.Success = false;
                response.Message = "Bu kullanıcıyı zaten takip ediyorsunuz.";
                return response;
            }

            var follow = new Follow
            {
                FollowerId = userId,
                FollowedId = targetId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.FollowRepository.AddAsync(follow);
            await _unitOfWork.Complete();

            // Bildirim oluştur
            await _notificationService.CreateNotificationAsync(
                targetId, // Bildirim alıcısı
                userId, // Bildirim göndereni
                "Follow",
                null, null, null,
                "sizi takip etmeye başladı."
            );

            response.Message = "Kullanıcı başarıyla takip edildi.";

            return response;
        }

        public async Task<ServiceResponse<object>> UnfollowUserAsync(int userId, int targetId)
        {
            var response = new ServiceResponse<object>();

            if (userId == targetId)
            {
                response.Success = false;
                response.Message = "Kendinizi takipten çıkaramazsınız.";
                return response;
            }

            var follow = await _unitOfWork.FollowRepository.GetFollowAsync(userId, targetId);

            if (follow == null)
            {
                response.Success = false;
                response.Message = "Bu kullanıcıyı zaten takip etmiyorsunuz.";
                return response;
            }

            _unitOfWork.FollowRepository.Delete(follow);
            await _unitOfWork.Complete();

            response.Message = "Kullanıcı takipten çıkarıldı.";

            return response;
        }

        public async Task<PagedList<UserDto>> GetUserFollowersAsync(int userId, UserParams userParams)
        {
            var followers = await _unitOfWork.UserRepository.GetUserFollowersAsync(userId, userParams);
            var followerDtos = _mapper.Map<PagedList<UserDto>>(followers);

            return followerDtos;
        }

        public async Task<PagedList<UserDto>> GetUserFollowingAsync(int userId, UserParams userParams)
        {
            var following = await _unitOfWork.UserRepository.GetUserFollowingAsync(userId, userParams);
            var followingDtos = _mapper.Map<PagedList<UserDto>>(following);

            return followingDtos;
        }

        public async Task<PagedList<UserDto>> SearchUsersAsync(string searchTerm, UserParams userParams)
        {
            var users = await _unitOfWork.UserRepository.SearchUsersAsync(searchTerm, userParams);
            var userDtos = _mapper.Map<PagedList<UserDto>>(users);

            return userDtos;
        }

        public async Task CreateUserAsync(string email, string firstName, string lastName)
        {
            // Burada kullanıcı oluşturma işlemleri yapılır
            // Örnek olarak event publish ediyoruz

            var userCreatedEvent = new UserCreatedEvent
            {
                UserId = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(userCreatedEvent);
            _logger.LogInformation($"User created event published for: {email}");
        }
    }
}
