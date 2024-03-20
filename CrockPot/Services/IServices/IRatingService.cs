using CrockPot.Models;

namespace CrockPot.Services.IServices
{
    public interface IRatingService
    {
        Task<List<Rating>> GetRatingsAsync();
        Task<List<Rating>> GetRatingsByRecipeIdAsync(int recipeId);
        Task<float> GetAverageRatingByRecipeIdAsync(int recipeId);
        Task<List<(Recipe recipe, float averageRating)>> GetHighestRatedRecipesAsync();
        Task<Rating> GetUserRatingOnRecipeAsync(string userId, int recipeId);
        Task<Rating> GetRatingByIdAsync(int id);
        Task<bool> SubmitRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(int id);
        bool RatingExists(int id);
    }
}
