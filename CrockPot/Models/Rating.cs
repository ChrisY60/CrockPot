using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Rating
    {
        public int Id { get; private set; }

        public string AuthorId { get; set; }

        public int? RecipeId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; }

        public Recipe? Recipe { get; set; }

    }
}
