namespace CrockPot.Models
{
    public class SharedRecipe
    {
        public int Id { get; private set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int RecipeId { get; set; }
        public DateTime Timestamp { get; set; }
        public Recipe Recipe { get; set; }
    }
}
