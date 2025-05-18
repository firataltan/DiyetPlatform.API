using AutoMapper;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;
 
using Microsoft.EntityFrameworkCore;
using DiyetPlatform.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;

        public PostService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment,
            IConfiguration config,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _config = config;
            _notificationService = notificationService;
        }

        public async Task<PagedList<PostResponseDto>> GetPostsAsync(DiyetPlatform.Core.Common.PostParams postParams)
        {
            var query = _unitOfWork.Posts.GetPostsQuery();

            // Apply filters
            if (!string.IsNullOrEmpty(postParams.Search))
                query = query.Where(p => p.Content.ToLower().Contains(postParams.Search.ToLower()));

            // Apply sorting
            query = (postParams.OrderBy) switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((postParams.PageNumber - 1) * postParams.PageSize)
                .Take(postParams.PageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

            return new PagedList<PostResponseDto>(
                postDtos,
                totalCount,
                postParams.PageNumber,
                postParams.PageSize);
        }

        public async Task<PostResponseDto> GetPostByIdAsync(int id)
        {
            var post = await _unitOfWork.Posts.GetPostByIdAsync(id);

            if (post == null)
                return null;

            return _mapper.Map<PostResponseDto>(post);
        }

        public async Task<PagedList<PostResponseDto>> GetUserPostsAsync(int userId, DiyetPlatform.Core.Common.PostParams postParams)
        {
            var query = _unitOfWork.Posts.GetPostsQuery()
                .Where(p => p.UserId == userId);

            // Apply sorting
            query = (postParams.OrderBy) switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((postParams.PageNumber - 1) * postParams.PageSize)
                .Take(postParams.PageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

            return new PagedList<PostResponseDto>(
                postDtos,
                totalCount,
                postParams.PageNumber,
                postParams.PageSize);
        }

        public async Task<PagedList<PostResponseDto>> GetUserFeedAsync(int userId, DiyetPlatform.Core.Common.PostParams postParams)
        {
            // Get following users
            var followingQuery = _unitOfWork.Follows.GetFollowsQuery()
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId);

            var followingIds = await followingQuery.ToListAsync();
            followingIds.Add(userId); // Add own posts too

            // Get posts from followed users
            var query = _unitOfWork.Posts.GetPostsQuery()
                .Where(p => followingIds.Contains(p.UserId));

            // Apply sorting
            query = (postParams.OrderBy) switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((postParams.PageNumber - 1) * postParams.PageSize)
                .Take(postParams.PageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

            return new PagedList<PostResponseDto>(
                postDtos,
                totalCount,
                postParams.PageNumber,
                postParams.PageSize);
        }

        public async Task<ServiceResponse<PostResponseDto>> CreatePostAsync(int userId, PostCreateDto postDto)
        {
            var response = new ServiceResponse<PostResponseDto>();

            try
            {
                var post = _mapper.Map<Post>(postDto);
                post.UserId = userId;
                post.CreatedAt = DateTime.UtcNow;

                // Handle media upload if provided
                if (postDto.Image != null)
                {
                    post.ImageUrl = await _unitOfWork.Posts.UploadImageAsync(postDto.Image);
                }

                _unitOfWork.Posts.Add(post);
                await _unitOfWork.Complete();

                var createdPost = await _unitOfWork.Posts.GetPostByIdAsync(post.Id);
                response.Data = _mapper.Map<PostResponseDto>(createdPost);
                response.Message = "Gönderi başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Gönderi oluşturulurken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<PostResponseDto>> UpdatePostAsync(int userId, int postId, PostCreateDto postDto)
        {
            var response = new ServiceResponse<PostResponseDto>();

            try
            {
                var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);

                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Gönderi bulunamadı.";
                    return response;
                }

                if (post.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bu gönderiyi düzenleme yetkiniz yok.";
                    return response;
                }

                // Update content
                post.Content = postDto.Content;
                post.UpdatedAt = DateTime.UtcNow;

                // Handle media upload if provided
                if (postDto.Image != null)
                {
                    // Delete old media if exists
                    if (!string.IsNullOrEmpty(post.ImageUrl))
                    {
                        await _unitOfWork.Posts.DeleteImageAsync(post.ImageUrl);
                    }

                    // Upload new media
                    post.ImageUrl = await _unitOfWork.Posts.UploadImageAsync(postDto.Image);
                }

                _unitOfWork.Posts.Update(post);
                await _unitOfWork.Complete();

                var updatedPost = await _unitOfWork.Posts.GetPostByIdAsync(postId);
                response.Data = _mapper.Map<PostResponseDto>(updatedPost);
                response.Message = "Gönderi başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Gönderi güncellenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> DeletePostAsync(int userId, int postId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);

                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Gönderi bulunamadı.";
                    return response;
                }

                if (post.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bu gönderiyi silme yetkiniz yok.";
                    return response;
                }

                // Delete media if exists
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    await _unitOfWork.Posts.DeleteImageAsync(post.ImageUrl);
                }

                _unitOfWork.Posts.Delete(post);
                await _unitOfWork.Complete();

                response.Message = "Gönderi başarıyla silindi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Gönderi silinirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> LikePostAsync(int userId, int postId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);

                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Gönderi bulunamadı.";
                    return response;
                }

                var isLiked = await _unitOfWork.Posts.IsPostLikedByUserAsync(userId, postId);
                if (isLiked)
                {
                    response.Success = false;
                    response.Message = "Bu gönderiyi zaten beğendiniz.";
                    return response;
                }

                var like = new Like
                {
                    UserId = userId,
                    PostId = postId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Posts.AddLikeAsync(like);
                await _unitOfWork.Complete();

                // Send notification to post owner
                if (userId != post.UserId)
                {
                    await _notificationService.CreateNotificationAsync(
                        post.UserId,
                        userId,
                        $"{userId} gönderinizi beğendi.",
                        "like",
                        postId: postId);
                }

                response.Message = "Gönderi beğenildi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Gönderi beğenilirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> UnlikePostAsync(int userId, int postId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);

                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Gönderi bulunamadı.";
                    return response;
                }

                var like = await _unitOfWork.Posts.GetPostLikeAsync(userId, postId);
                if (like == null)
                {
                    response.Success = false;
                    response.Message = "Bu gönderiyi henüz beğenmediniz.";
                    return response;
                }

                _unitOfWork.Posts.DeleteLike(like);
                await _unitOfWork.Complete();

                response.Message = "Gönderi beğenisi kaldırıldı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Gönderi beğenisi kaldırılırken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int postId, CommentCreateDto commentCreateDto)
        {
            var response = new ServiceResponse<CommentDto>();

            try
            {
                var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);

                if (post == null)
                {
                    response.Success = false;
                    response.Message = "Gönderi bulunamadı.";
                    return response;
                }

                var comment = new Comment
                {
                    Content = commentCreateDto.Content,
                    UserId = userId,
                    PostId = postId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Posts.AddCommentAsync(comment);
                await _unitOfWork.Complete();

                // Get user info for response
                var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

                // Create response DTO
                var resultCommentDto = new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UserId = userId,
                    Username = user.Username,
                    UserProfileImage = user.Profile?.ProfileImageUrl
                };

                response.Data = resultCommentDto;
                response.Message = "Yorum başarıyla eklendi.";

                // Send notification to post owner
                if (userId != post.UserId)
                {
                    await _notificationService.CreateNotificationAsync(
                        post.UserId,
                        userId,
                        $"{userId} gönderinize yorum yaptı.",
                        "comment",
                        postId: postId);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Yorum eklenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }
        public async Task<PagedList<PostResponseDto>> SearchPostsAsync(string searchTerm, DiyetPlatform.Core.Common.PostParams postParams)
        {
            var query = _unitOfWork.Posts.GetPostsQuery()
                .Where(p => p.Content.ToLower().Contains(searchTerm.ToLower()));

            // Apply sorting
            query = (postParams.OrderBy) switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((postParams.PageNumber - 1) * postParams.PageSize)
                .Take(postParams.PageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

            return new PagedList<PostResponseDto>(
                postDtos,
                totalCount,
                postParams.PageNumber,
                postParams.PageSize);
        }

        public async Task<PagedList<PostResponseDto>> GetFollowingPostsAsync(int userId, DiyetPlatform.Core.Common.PostParams postParams)
        {
            // Get following users
            var followingQuery = _unitOfWork.Follows.GetFollowsQuery()
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId);

            var followingIds = await followingQuery.ToListAsync();

            // Get posts from followed users
            var query = _unitOfWork.Posts.GetPostsQuery()
                .Where(p => followingIds.Contains(p.UserId));

            // Apply sorting
            query = (postParams.OrderBy) switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((postParams.PageNumber - 1) * postParams.PageSize)
                .Take(postParams.PageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

            return new PagedList<PostResponseDto>(
                postDtos,
                totalCount,
                postParams.PageNumber,
                postParams.PageSize);
        }

        public async Task<PagedList<PostResponseDto>> GetLikedPostsAsync(int userId, DiyetPlatform.Core.Common.PostParams postParams)
        {
            // Get likes for the user
            var likedPostIds = _unitOfWork.Likes.GetLikesQuery()
                .Where(l => l.UserId == userId && l.PostId.HasValue)
                .Select(l => l.PostId.Value);

            // Get the liked posts
            var query = _unitOfWork.Posts.GetPostsQuery()
                .Where(p => likedPostIds.Contains(p.Id));

            // Apply sorting
            query = (postParams.OrderBy) switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var posts = await query
                .Skip((postParams.PageNumber - 1) * postParams.PageSize)
                .Take(postParams.PageSize)
                .ToListAsync();

            var postDtos = _mapper.Map<List<PostResponseDto>>(posts);

            return new PagedList<PostResponseDto>(
                postDtos,
                totalCount,
                postParams.PageNumber,
                postParams.PageSize);
        }
    }
}