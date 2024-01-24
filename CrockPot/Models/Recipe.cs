using System.ComponentModel.DataAnnotations;

namespace CrockPot.Models
{
    public class Recipe
    {
        private int _id;
        private string _name;
        private string _description;
        private ICollection<Ingredient> _ingredients = new List<Ingredient>();
        private string? _authorId;
        private ICollection<Category> _categories = new List<Category>();
        private string? _imageUrl;

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

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ICollection<Ingredient> Ingredients
        {
            get { return _ingredients; }
            set { _ingredients = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        public string? AuthorId
        {
            get { return _authorId; }
            set { _authorId = value; }
        }

        public ICollection<Category> Categories
        {
            get { return _categories; }
            set { _categories = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        public string? ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }
    }
}
