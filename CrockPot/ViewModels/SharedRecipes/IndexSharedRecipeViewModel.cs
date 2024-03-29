﻿using CrockPot.Models;

namespace CrockPot.ViewModels.SharedRecipes
{
    public class IndexSharedRecipeViewModel
    {
        public List<SharedRecipe> SharedRecipes { get; set; }
        public Dictionary<string, string> SendersNames { get; set; }
        public Dictionary<int, string> TimeDifference { get; set; }
    }
}
