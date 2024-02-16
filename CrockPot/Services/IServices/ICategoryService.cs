using CrockPot.Models;

namespace CrockPot.Services.IServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> IsCategoryNameUniqueAsync(string name);
        bool CategoryExists(int id);
    }

}
