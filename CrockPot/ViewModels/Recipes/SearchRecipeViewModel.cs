using CrockPot.Models;

namespace CrockPot.ViewModels.Recipes
{
    public class SearchRecipeViewModel
    {
        public List<Category>? AllCategories { get; set; }
        public List<Ingredient>? AllIngredients { get; set; }
        public string? Name { get; set; }
        public int[]? SelectedCategories { get; set; }
        public int[]? SelectedIngredients { get; set; }

    }
}
