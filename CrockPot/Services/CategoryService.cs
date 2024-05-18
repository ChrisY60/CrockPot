using CrockPot.Data;
using CrockPot.Models;
using CrockPot.Services.IServices;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CrockPot.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> CreateCategoryAsync(Category category, ModelStateDictionary modelState)
        {
            try
            {
                if (!await IsCategoryNameUniqueAsync(category.Name))
                {
                    modelState.AddModelError("Name", "A category with this name already exists.");
                    return false;
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                modelState.AddModelError(string.Empty, "Failed to create the category. Please try again.");
                return false;
            }
        }


        public async Task<bool> UpdateCategoryAsync(Category category, ModelStateDictionary modelState)
        {

            try
            {
                if(!await IsCategoryNameUniqueAsync(category.Name))
                {
                    modelState.AddModelError("Name", "A category with this name already exists.");
                    return false;
                }
                _context.Update(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                Category? category = await _context.Categories.FindAsync(id);
                if (category != null)
                {
                    _context.Categories.Remove(category);
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

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name)
        {
            return await _context.Categories.AllAsync(c => c.Name != name);
        }
    }
}
