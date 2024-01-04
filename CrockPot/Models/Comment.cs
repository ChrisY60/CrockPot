namespace CrockPot.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; }
    }
}
