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
        public string ImageUrl { get; set; }

        public Recipe(int id, string name, string description, string authorId)
        {
            Id = id;
            Name = name;
            Description = description;
            AuthorId = authorId;
        }

        public Recipe() { }
    }
}
