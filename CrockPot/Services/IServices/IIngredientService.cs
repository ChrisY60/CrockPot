using CrockPot.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrockPot.Services.IServices
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetIngredientsAsync();
        Task<Ingredient> GetIngredientByIdAsync(int id);
        Task<bool> CreateIngredientAsync(Ingredient ingredient, ModelStateDictionary modelState);
        Task<bool> UpdateIngredientAsync(Ingredient ingredient, ModelStateDictionary modelState);
        Task<bool> DeleteIngredientAsync(int id);

        Task<bool> IsIngredientNameUniqueAsync(string name);

        bool IngredientExists(int id);
    }
}
