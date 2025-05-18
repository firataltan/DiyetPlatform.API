using DiyetPlatform.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface ISavedRecipeRepository : IGenericRepository<SavedRecipe>
    {
        Task<SavedRecipe> GetSavedRecipeAsync(int userId, int recipeId);
        IQueryable<SavedRecipe> GetSavedRecipesQuery();
        Task<bool> IsRecipeSavedByUserAsync(int userId, int recipeId);
        new void Delete(SavedRecipe savedRecipe);
    }
} 