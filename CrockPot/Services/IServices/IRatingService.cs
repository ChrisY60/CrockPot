using CrockPot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrockPot.Services.IServices
{
    public interface IRatingService
    {
        Task<List<Rating>> GetRatingsAsync();
        Task<List<Rating>> GetRatingsByRecipeIdAsync(int recipeId);
        Task<float> GetAverageRatingByRecipeIdAsync(int recipeId);
        Task<List<Rating>> GetRatingsByAuthorIdAsync(string authorId);
        Task<Rating> GetRatingByIdAsync(int id);
        Task<bool> CreateRatingAsync(Rating rating);
        Task<bool> UpdateRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(int id);
        bool RatingExists(int id);
    }
}
