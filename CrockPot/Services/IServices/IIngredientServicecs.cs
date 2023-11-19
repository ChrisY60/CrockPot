using CrockPot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrockPot.Services.IServices
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetIngredientsAsync();
        Task<Ingredient> GetIngredientByIdAsync(int id);
        Task<bool> CreateIngredientAsync(Ingredient ingredient);
        Task<bool> UpdateIngredientAsync(Ingredient ingredient);
        Task<bool> DeleteIngredientAsync(int id);
        bool IngredientExists(int id);
    }
}
