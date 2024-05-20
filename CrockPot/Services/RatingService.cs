using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<bool> SubmitRatingAsync(Rating rating, string currentUser, ModelStateDictionary modelState)
        {
            try
            {
                Rating? existingRating = await GetUserRatingOnRecipeAsync(currentUser, rating.RecipeId);

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
                modelState.AddModelError(string.Empty, "Failed to create the rating. Please try again.");
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
            List<Recipe>? recipes = await _context.Recipes.ToListAsync();
            List<(Recipe, float)>? topRatedRecipes = new List<(Recipe, float)>();

            foreach (var recipe in recipes)
            {
                float averageRating = await GetAverageRatingByRecipeIdAsync(recipe.Id);
                topRatedRecipes.Add((recipe, averageRating));
            }

            topRatedRecipes.Sort(((Recipe, float) x, (Recipe, float) y) => y.Item2.CompareTo(x.Item2));

            return topRatedRecipes.Take(10).ToList();
        }


        private async Task<Dictionary<int, float>> GetAverageRatingsForCategoriesAsync(string userId)
        {
            Dictionary<int, float> categoryRatings = new Dictionary<int, float>();

            List<Rating>? userRatings = await _context.Ratings
                                            .Where(r => r.AuthorId == userId)
                                            .Include(r => r.Recipe)
                                            .ThenInclude(r => r.Categories)
                                            .ToListAsync();

            Dictionary<int, (float sum, int count)> categoryRatingSums = new Dictionary<int, (float sum, int count)>();

            foreach (Rating rating in userRatings)
            {
                foreach (Category category in rating.Recipe.Categories)
                {
                    if (!categoryRatingSums.ContainsKey(category.Id))
                    {
                        categoryRatingSums[category.Id] = (0, 0);
                    }

                    categoryRatingSums[category.Id] = (
                        categoryRatingSums[category.Id].sum + rating.RatingValue,
                        categoryRatingSums[category.Id].count + 1
                    );
                }
            }

            foreach (KeyValuePair<int, (float sum, int count)> entry in categoryRatingSums)
            {
                categoryRatings[entry.Key] = entry.Value.sum / entry.Value.count;
            }

            return categoryRatings;
        }

        private async Task<Dictionary<int, float>> GetAverageRatingsForIngredientsAsync(string userId)
        {
            Dictionary<int, float>? ingredientRatings = new Dictionary<int, float>();

            List<Rating>? userRatings = await _context.Ratings
                                            .Where(r => r.AuthorId == userId)
                                            .Include(r => r.Recipe)
                                            .ThenInclude(r => r.Ingredients)
                                            .ToListAsync();

            Dictionary<int, (float sum, int count)>? ingredientRatingSums = new Dictionary<int, (float sum, int count)>();

            foreach (Rating rating in userRatings)
            {
                foreach (Ingredient ingredient in rating.Recipe.Ingredients)
                {
                    if (!ingredientRatingSums.ContainsKey(ingredient.Id))
                    {
                        ingredientRatingSums[ingredient.Id] = (0, 0);
                    }

                    ingredientRatingSums[ingredient.Id] = (
                        ingredientRatingSums[ingredient.Id].sum + rating.RatingValue,
                        ingredientRatingSums[ingredient.Id].count + 1
                    );
                }
            }

            foreach (KeyValuePair<int, (float sum, int count)> entry in ingredientRatingSums)
            {
                ingredientRatings[entry.Key] = entry.Value.sum / entry.Value.count;
            }

            return ingredientRatings;
        }

        public async Task<List<(Recipe recipe, float averageRating)>> GetRecommendedRecipesAsync(string userId)
        {
            Dictionary<int, float> categoryRatings = await GetAverageRatingsForCategoriesAsync(userId);
            Dictionary<int, float> ingredientRatings = await GetAverageRatingsForIngredientsAsync(userId);

            var topCategories = categoryRatings.OrderByDescending(cr => cr.Value)
                                               .Take(10)
                                               .Select(cr => cr.Key)
                                               .ToList();

            var topIngredients = ingredientRatings.OrderByDescending(ir => ir.Value)
                                                  .Take(10)
                                                  .Select(ir => ir.Key)
                                                  .ToList();

            var recommendedRecipesWithRatings = new List<(Recipe recipe, float averageRating)>();

            var recommendedRecipes = await _context.Recipes
                                                    .Include(r => r.Categories)
                                                    .Include(r => r.Ingredients)
                                                    .ToListAsync();

            foreach (var recipe in recommendedRecipes)
            {
                float ingredientRatingSum = 0;
                int ingredientRatingCount = 0;

                foreach (var ingredient in recipe.Ingredients)
                {
                    if (ingredientRatings.TryGetValue(ingredient.Id, out var rating))
                    {
                        ingredientRatingSum += rating;
                        ingredientRatingCount++;
                    }
                }

                float categoryRatingSum = 0;
                int categoryRatingCount = 0;

                foreach (Category category in recipe.Categories)
                {
                    if (categoryRatings.TryGetValue(category.Id, out var rating))
                    {
                        categoryRatingSum += rating;
                        categoryRatingCount++;
                    }
                }

                float averageRating = 0;
                if (ingredientRatingCount > 0 || categoryRatingCount > 0)
                {
                    averageRating = (ingredientRatingSum + categoryRatingSum) / (ingredientRatingCount + categoryRatingCount);
                    averageRating = (float)Math.Round(averageRating, 2);
                }

                recommendedRecipesWithRatings.Add((recipe, averageRating));
            }

            recommendedRecipesWithRatings.Sort((x, y) => y.averageRating.CompareTo(x.averageRating));

            return recommendedRecipesWithRatings.Take(3).ToList();
        }





    }
}
