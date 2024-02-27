using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Comment
    {
        public int Id { get; private set; }
        public int RecipeId { get; set; }
        public string AuthorId { get; set; }

        [MinLength(1)]
        [MaxLength(500)]
        public string Content { get; set; }
        public Recipe? Recipe { get; set; }

    }
}
