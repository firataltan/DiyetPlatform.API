using AutoMapper;
using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
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

        public async Task<PagedList<PostResponseDto>> GetPostsAsync(PostParams postParams)
        {
            var posts = await _unitOfWork.PostRepository.GetPostsAsync(postParams);
            var postDtos = _mapper.Map<PagedList<PostResponseDto>>(posts);

            return postDtos;
        }

        public async Task<PostResponseDto> GetPostByIdAsync(int id)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(id);

            if (post == null)
                return null;

            return _mapper.Map<PostResponseDto>(post, opts => opts.Items["currentUserId"] = post.UserId);
        }

        public async Task<PagedList<PostResponseDto>> GetUserPostsAsync(int userId, PostParams postParams)
        {
            var posts = await _unitOfWork.PostRepository.GetUserPostsAsync(userId, postParams);
            var postDtos = _mapper.Map<PagedList<PostResponseDto>>(posts);

            return postDtos;
        }

        public async Task<PagedList<PostResponseDto>> GetUserFeedAsync(int userId, PostParams postParams)
        {
            var posts = await _unitOfWork.PostRepository.GetUserFeedAsync(userId, postParams);
            var postDtos = _mapper.Map<PagedList<PostResponseDto>>(posts);

            return postDtos;
        }

        public async Task<PostResponseDto> CreatePostAsync(int userId, PostCreateDto postDto)
        {
            var post = _mapper.Map<Post>(postDto);
            post.UserId = userId;
            post.CreatedAt = DateTime.UtcNow;

            // Medya yükleme
            if (postDto.Media != null)
            {
                var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "post-media");

                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileExtension = Path.GetExtension(postDto.Media.FileName);
                var fileName = $"{userId}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.Media.CopyToAsync(fileStream);
                }

                post.MediaUrl = $"/uploads/post-media/{fileName}";
            }

            await _unitOfWork.PostRepository.AddAsync(post);
            await _unitOfWork.Complete();

            // Gönderiyi tüm detaylarıyla yeniden yükle
            var createdPost = await _unitOfWork.PostRepository.GetPostByIdAsync(post.Id);

            return _mapper.Map<PostResponseDto>(createdPost, opts => opts.Items["currentUserId"] = userId);
        }

        public async Task<ServiceResponse<PostResponseDto>> UpdatePostAsync(int userId, int postId, PostCreateDto postDto)
        {
            var response = new ServiceResponse<PostResponseDto>();

            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

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

            // İçeriği güncelle
            post.Content = postDto.Content;
            post.UpdatedAt = DateTime.UtcNow;

            // Medya yükleme
            if (postDto.Media != null)
            {
                var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "post-media");

                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileExtension = Path.GetExtension(postDto.Media.FileName);
                var fileName = $"{userId}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await postDto.Media.CopyToAsync(fileStream);
                }

                // Eski medyayı sil (eğer varsa)
                if (!string.IsNullOrEmpty(post.MediaUrl))
                {
                    var oldMediaPath = Path.Combine(_hostEnvironment.WebRootPath, post.MediaUrl.TrimStart('/'));
                    if (File.Exists(oldMediaPath))
                    {
                        File.Delete(oldMediaPath);
                    }
                }

                post.MediaUrl = $"/uploads/post-media/{fileName}";
            }

            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.Complete();

            // Gönderiyi tüm detaylarıyla yeniden yükle
            var updatedPost = await _unitOfWork.PostRepository.GetPostByIdAsync(post.Id);

            response.Data = _mapper.Map<PostResponseDto>(updatedPost);
            response.Message = "Gönderi başarıyla güncellendi.";

            return response;
        }

        public async Task<ServiceResponse<object>> DeletePostAsync(int userId, int postId)
        {
            var response = new ServiceResponse<object>();

            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

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

            // Medyayı sil (eğer varsa)
            if (!string.IsNullOrEmpty(post.MediaUrl))
            {
                var mediaPath = Path.Combine(_hostEnvironment.WebRootPath, post.MediaUrl.TrimStart('/'));
                if (File.Exists(mediaPath))
                {
                    File.Delete(mediaPath);
                }
            }

            _unitOfWork.PostRepository.Delete(post);
            await _unitOfWork.Complete();

            response.Message = "Gönderi başarıyla silindi.";

            return response;
        }

        public async Task<ServiceResponse<object>> LikePostAsync(int userId, int postId)
        {
            var response = new ServiceResponse<object>();

            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

            if (post == null)
            {
                response.Success = false;
                response.Message = "Gönderi bulunamadı.";
                return response;
            }

            var isLiked = await _unitOfWork.PostRepository.IsPostLikedByUserAsync(userId, postId);

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

            await _unitOfWork.PostRepository.AddLikeAsync(like);
            await _unitOfWork.Complete();

            // Bildirim oluştur (kendi gönderisini beğenmediği sürece)
            if (post.UserId != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    post.UserId, // Bildirim alıcısı
                    userId, // Bildirim göndereni
                    "Like",
                    postId, null, null,
                    "gönderinizi beğendi."
                );
            }

            response.Message = "Gönderi başarıyla beğenildi.";

            return response;
        }

        public async Task<ServiceResponse<object>> UnlikePostAsync(int userId, int postId)
        {
            var response = new ServiceResponse<object>();

            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

            if (post == null)
            {
                response.Success = false;
                response.Message = "Gönderi bulunamadı.";
                return response;
            }

            var like = await _unitOfWork.PostRepository.GetPostLikeAsync(userId, postId);

            if (like == null)
            {
                response.Success = false;
                response.Message = "Bu gönderiyi zaten beğenmediniz.";
                return response;
            }

            _unitOfWork.PostRepository.DeleteLike(like);
            await _unitOfWork.Complete();

            response.Message = "Gönderi beğenisi kaldırıldı.";

            return response;
        }

        public async Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int postId, CommentCreateDto commentDto)
        {
            var response = new ServiceResponse<CommentDto>();

            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);

            if (post == null)
            {
                response.Success = false;
                response.Message = "Gönderi bulunamadı.";
                return response;
            }

            var comment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = commentDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PostRepository.AddCommentAsync(comment);
            await _unitOfWork.Complete();

            // Yorumun sahibi ve kullanıcı bilgilerini yükle
            comment = await _unitOfWork.PostRepository.Comments
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            // Bildirim oluştur (kendi gönderisine yorum yapmadığı sürece)
            if (post.UserId != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    post.UserId, // Bildirim alıcısı
                    userId, // Bildirim göndereni
                    "Comment",
                    postId, null, comment.Id,
                    "gönderinize yorum yaptı."
                );
            }

            response.Data = _mapper.Map<CommentDto>(comment);
            response.Message = "Yorum başarıyla eklendi.";

            return response;
        }

        public async Task<PagedList<PostResponseDto>> SearchPostsAsync(string searchTerm, PostParams postParams)
        {
            var posts = await _unitOfWork.PostRepository.SearchPostsAsync(searchTerm, postParams);
            var postDtos = _mapper.Map<PagedList<PostResponseDto>>(posts);

            return postDtos;
        }
    }
}