using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Ingredient>? Ingredients { get; set; } = new List<Ingredient>();
        public string AuthorId { get; set; }
        public ICollection<Category>? Categories { get; set; } = new List<Category>();

    }
}