using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CrockPot.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            return await _context.Recipes.ToListAsync();
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Categories)
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CreateRecipeAsync(Recipe recipe, int[] selectedCategories, int[] selectedIngredients)
        {
            if (selectedCategories != null)
            {
                recipe.Categories = _context.Categories.Where(c => selectedCategories.Contains(c.Id)).ToList();
            }

            if (selectedIngredients != null)
            {
                recipe.Ingredients = _context.Ingredients.Where(i => selectedIngredients.Contains(i.Id)).ToList();
            }

            _context.Add(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRecipeAsync(Recipe recipe)
        {
            _context.Update(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        public async Task<List<Recipe>> GetAllRecipesByFilterAsync(string name, int[] selectedCategories, int[] selectedIngredients)
        {
            IQueryable<Recipe> query = _context.Recipes;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(recipe => recipe.Name.Contains(name));
            }

            if (selectedCategories != null && selectedCategories.Length > 0)
            {
                query = query.Where(recipe => recipe.Categories.Any(category => selectedCategories.Contains(category.Id)));
            }

            if (selectedIngredients != null && selectedIngredients.Length > 0)
            {
                query = query.Where(recipe => recipe.Ingredients.All(ingredient => selectedIngredients.Contains(ingredient.Id)));
            }

            return await query.ToListAsync();

        }
    }
}
