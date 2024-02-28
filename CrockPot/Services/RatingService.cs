using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CrockPot.Services
{
    public class RatingService : IRatingService
    {
        private readonly ApplicationDbContext _context;

        public RatingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rating>> GetRatingsAsync()
        {
            return await _context.Ratings.ToListAsync();
        }

        public async Task<List<Rating>> GetRatingsByRecipeIdAsync(int recipeId)
        {
            return await _context.Ratings.Where(rating => rating.RecipeId == recipeId).ToListAsync();
        }

        public async Task<Rating> GetRatingByIdAsync(int id)
        {
            return await _context.Ratings.FindAsync(id);
        }

        public async Task<Rating> GetUserRatingOnRecipeAsync(string userId, int recipeId)
        {
            return await _context.Ratings.FirstOrDefaultAsync(r => r.AuthorId == userId && r.RecipeId == recipeId);
        }
        public async Task<float> GetAverageRatingByRecipeIdAsync(int recipeId)
        {
            var ratings = await GetRatingsByRecipeIdAsync(recipeId);

            if (ratings.Any())
            {
                float averageRating = (float)ratings.Select(r => r.RatingValue).Average();
                return averageRating;
            }

            return 0;
        }
        public async Task<bool> CreateRatingAsync(Rating rating)
        {
            try
            {
                var existingRating = await GetUserRatingOnRecipeAsync(rating.AuthorId, rating.RecipeId);

                if (existingRating != null)
                {
                    existingRating.RatingValue = rating.RatingValue;
                    _context.Ratings.Update(existingRating);
                }else{
                    _context.Ratings.Add(rating);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRatingAsync(int id)
        {
            try
            {
                var rating = await _context.Ratings.FindAsync(id);
                if (rating != null)
                {
                    _context.Ratings.Remove(rating);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }


        public bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }

        public async Task<List<(Recipe recipe, float averageRating)>> GetHighestRatedRecipesAsync()
        {
            var recipes = await _context.Recipes.ToListAsync();
            var topRatedRecipes = new List<(Recipe, float)>();

            foreach (var recipe in recipes)
            {
                var averageRating = await GetAverageRatingByRecipeIdAsync(recipe.Id);
                topRatedRecipes.Add((recipe, averageRating));
            }

            topRatedRecipes.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            return topRatedRecipes.Take(10).ToList();
        }

    }
}
