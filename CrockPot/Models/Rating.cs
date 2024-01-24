using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Rating
    {
        private int _id;
        private string _authorId;
        private int _recipeId;

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        private int _ratingValue;

        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public string AuthorId
        {
            get { return _authorId; }
            set { _authorId = value; }
        }

        public int RecipeId
        {
            get { return _recipeId; }
            set { _recipeId = value; }
        }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue
        {
            get { return _ratingValue; }
            set { _ratingValue = value; }
        }
    }
}
