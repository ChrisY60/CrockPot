using CrockPot.Models;
using CrockPot.ViewModels.Recipes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace CrockPot.Services.IServices
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<bool> CreateRecipeAsync(CreateRecipeViewModel viewModel, string currentUser, ModelStateDictionary modelState);
        Task<bool> UpdateRecipeAsync(EditRecipeViewModel viewModel, string currentUser, ModelStateDictionary modelState);
        Task<bool> DeleteRecipeAsync(int id, ModelStateDictionary modelState);
        bool RecipeExists(int id);
        Task<Dictionary<string, string>> GetAuthorsNames(List<Recipe> Recipes);
        Task<List<Recipe>> GetAllRecipesByFilterAsync(string name, int[] selectedCategories, int[] selectedIngredients);
        Task<DetailsRecipeViewModel> GetDetailsViewModelByRecipeId(int id, string currentUser);
        Task<EditRecipeViewModel> GetEditViewModelByRecipeId(int recipeId);
    }
}
