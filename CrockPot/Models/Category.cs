using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Category
    {
        private int _id;
        private string _name;
        private ICollection<Recipe>? _recipes;

        [Key]
        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ICollection<Recipe>? Recipes
        {
            get { return _recipes; }
            set { _recipes = value; }
        }
    }
}
