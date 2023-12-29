using CrockPot.Models;

namespace CrockPot.Services.IServices
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<bool> CreateRecipeAsync(Recipe recipe, int[] selectedCategories, int[] selectedIngredients);
        Task<bool> UpdateRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(int id);
        bool RecipeExists(int id);

        Task<List<Recipe>> GetAllRecipesByFilterAsync(string name, int[] selectedCategories, int[] selectedIngredients);
    }
}
