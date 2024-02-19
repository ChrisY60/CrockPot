using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CrockPot.Services
{
    public class SharedRecipeService : ISharedRecipeService
    {
        private readonly ApplicationDbContext _context;

        public SharedRecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateSharedRecipeAsync(SharedRecipe newRecipe)
        {
            try
            {
                _context.SharedRecipes.Add(newRecipe);
                await _context.SaveChangesAsync();
            }catch (DbUpdateException)
            {
                return false;
            }
            
            return true;
        }

        public async Task<List<SharedRecipe>> GetSharedRecipesAsync()
        {
            return await _context.SharedRecipes.ToListAsync();
        }

        public async Task<List<SharedRecipe>> GetSharedRecipesByReceiverAsync(string receiverId)
        {
            return await _context.SharedRecipes
                .Include(sr => sr.RecipeObject)
                .Where(sr => sr.ReceiverId == receiverId)
                .ToListAsync();
        }
    }
}
