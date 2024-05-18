namespace CrockPot.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Recipe>? Recipes { get; set; }

        public Ingredient(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public Ingredient() { }
    }
}
