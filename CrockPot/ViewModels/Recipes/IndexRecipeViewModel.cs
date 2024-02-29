using CrockPot.Models;

namespace CrockPot.ViewModels.Recipes
{
    public class IndexRecipeViewModel
    {
        public IEnumerable<Recipe> Recipes { get; set; }
        public Dictionary<string, string> AuthorNames { get; set; }
    }
}
