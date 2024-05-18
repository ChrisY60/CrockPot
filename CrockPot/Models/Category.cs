namespace CrockPot.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Category(){}

    }
}