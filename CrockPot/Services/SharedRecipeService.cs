﻿using CrockPot.Data;
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
            _context.SharedRecipes.Add(newRecipe);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SharedRecipe>> GetSharedRecipesAsync()
        {
            return await _context.SharedRecipes.ToListAsync();
        }

        public async Task<List<SharedRecipe>> GetSharedRecipesByReceiverAsync(string receiverId)
        {
            return await _context.SharedRecipes
                .Where(recipe => recipe.ReceiverId == receiverId)
                .ToListAsync();
        }
    }
}
