using CrockPot.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CrockPot.Services.IServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(Category category, ModelStateDictionary modelState);
        Task<bool> UpdateCategoryAsync(Category category, ModelStateDictionary modelState);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> IsCategoryNameUniqueAsync(string name);
        bool CategoryExists(int id);
    }

}
