using DiyetPlatform.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        new Task<Category> GetByIdAsync(int id);
        IQueryable<Category> GetCategoriesQuery();
        Task<string> UploadImageAsync(IFormFile image);
        Task DeleteImageAsync(string imageUrl);
        new void Add(Category category);
        new void Update(Category category);
        new void Delete(Category category);
    }
}