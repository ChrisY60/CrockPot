using CrockPot.Models;

namespace CrockPot.ViewModels.SharedRecipes
{
    public class IndexSharedRecipeViewModel
    {
        public List<SharedRecipe> SharedRecipes { get; set; }
        public Dictionary<string, string> SenderNames { get; set; }
    }
}

