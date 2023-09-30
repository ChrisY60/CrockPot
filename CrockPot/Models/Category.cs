using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Recipe>? Recipes { get; set; }

    }
}
