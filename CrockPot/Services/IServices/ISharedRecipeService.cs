using CrockPot.Models;
using CrockPot.ViewModels.SharedRecipes;

namespace CrockPot.Services.IServices
{
    public interface ISharedRecipeService
    {
        Task<List<SharedRecipe>> GetSharedRecipesAsync();
        Task<List<SharedRecipe>> GetSharedRecipesByReceiverAsync(string receiverId);
        Task<bool> CreateSharedRecipeAsync(SharedRecipe sharedRecipe);
        string CalculateTimeDifference(DateTime startDateTime, DateTime endDateTime);
        Task<Dictionary<string, string>> GetSenderNames(List<SharedRecipe> sharedRecipes);
        Task<IndexSharedRecipeViewModel> GetIndexViewModel(string currentUser);


    }
}
