namespace CrockPot.Models
{
    public class Ingredient
    {
        private int _id;
        private string _name;
        private ICollection<Recipe>? _recipes;

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
