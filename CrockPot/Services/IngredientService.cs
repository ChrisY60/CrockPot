using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CrockPot.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ingredient>> GetIngredientsAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(int id)
        {
            return await _context.Ingredients.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CreateIngredientAsync(Ingredient ingredient, ModelStateDictionary modelState)
        {
            try
            {
                if (!await IsIngredientNameUniqueAsync(ingredient.Name))
                {
                    modelState.AddModelError("Name", "An ingredient with this name already exists.");
                    return false;
                }
                _context.Add(ingredient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to create the ingredient. Please try again.");
                return false;
            }
        }

        public async Task<bool> UpdateIngredientAsync(Ingredient ingredient, ModelStateDictionary modelState)
        {
            try
            {
                if (!await IsIngredientNameUniqueAsync(ingredient.Name))
                {
                    modelState.AddModelError("Name", "An ingredient with this name already exists.");
                    return false;
                }
                _context.Update(ingredient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to create the ingredient. Please try again.");
                return false;
            }
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            try
            {
                Ingredient? ingredient = await _context.Ingredients.FindAsync(id);
                if (ingredient != null)
                {
                    _context.Ingredients.Remove(ingredient);
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


        public bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }

        public async Task<bool> IsIngredientNameUniqueAsync(string name){
            return await _context.Ingredients.AllAsync(i => i.Name != name);
        }
    }
}
