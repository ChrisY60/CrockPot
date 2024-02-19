using CrockPot.Models;
using Microsoft.AspNetCore.Identity;

namespace CrockPot.ViewModels
{
    public class DetailsRecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public string AuthorName { get; set; }
        public List<string> FormattedComments { get; set; }
        public double AverageRating { get; set; }
        public Rating CurrentRating { get; set; }
        public List<IdentityUser> AllUsers { get; set; }
    }
}
