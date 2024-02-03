using CrockPot.Models;

namespace CrockPot.Services.IServices
{
    public interface ISharedRecipeService
    {
        Task<List<SharedRecipe>> GetSharedRecipesAsync();
        Task<List<SharedRecipe>> GetSharedRecipesByReceiverAsync(string receiverId);
        Task<bool> CreateSharedRecipeAsync(SharedRecipe newRecipe);
    }
}
