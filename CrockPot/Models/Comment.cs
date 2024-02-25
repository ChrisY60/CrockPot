using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Comment
    {
        public int Id { get; private set; }
        public int RecipeId { get; set; }
        public string AuthorId { get; set; }

        [MaxLength(500)]
        public string Content { get; set; }
        public Recipe Recipe { get; set; }

        public Comment(int recipeId, string content)
        {
            RecipeId = recipeId;
            Content = content;
        }

    }
}
