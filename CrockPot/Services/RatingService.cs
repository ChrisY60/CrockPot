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
            return await _context.Ratings
                .Where(rating => rating.RecipeId == recipeId)
                .ToListAsync();
        }

        public async Task<List<Rating>> GetRatingsByAuthorIdAsync(string authorId)
        {
            return await _context.Ratings
                .Where(rating => rating.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task<Rating> GetRatingByIdAsync(int id)
        {
            return await _context.Ratings.FindAsync(id);
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
            _context.Add(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRatingAsync(Rating rating)
        {
            _context.Update(rating);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRatingAsync(int id)
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

        public bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
