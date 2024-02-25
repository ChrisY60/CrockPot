namespace CrockPot.Models
{
    public class Recipe
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public string AuthorId { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Rating> Ratings { get; set; }

        public string ImageUrl { get; set; }

        public Recipe() { }
    }
}
