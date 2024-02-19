using CrockPot.Models;
using System.ComponentModel.DataAnnotations;

namespace CrockPot.ViewModels.Recipes
{
    public class CreateRecipeViewModel
    {
        [Required(ErrorMessage = "Please enter the recipe name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the recipe description")]
        public string Description { get; set; }

        [Display(Name = "Categories")]
        public int[] SelectedCategories { get; set; }

        [Display(Name = "Ingredients")]
        public int[] SelectedIngredients { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }

        public List<Category>? AllCategories { get; set; }
        public List<Ingredient>? AllIngredients { get; set; }

    }
}
