using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly IHostEnvironment _hostEnvironment;

        public CategoryRepository(ApplicationDbContext context, IHostEnvironment hostEnvironment) : base(context)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Category> GetCategoriesQuery()
        {
            return _context.Categories
                .Include(c => c.Recipes)
                .AsQueryable();
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Recipes)
                .ToListAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            var uploadsFolderPath = Path.Combine(_hostEnvironment.ContentRootPath, "uploads", "category-images");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var fileExtension = Path.GetExtension(image.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/uploads/category-images/{fileName}";
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, imageUrl.TrimStart('/'));

            if (File.Exists(imagePath))
            {
                await Task.Run(() => File.Delete(imagePath));
            }
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public new void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        public void Remove(Category entity)
        {
            _context.Categories.Remove(entity);
        }

        public void AddRange(IEnumerable<Category> entities)
        {
            _context.Categories.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<Category> entities)
        {
            await _context.Categories.AddRangeAsync(entities);
        }

        public void RemoveRange(IEnumerable<Category> entities)
        {
            _context.Categories.RemoveRange(entities);
        }
    }
} 