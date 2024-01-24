namespace CrockPot.Models
{
    public class Comment
    {
        private int _id;
        private int _recipeId;
        private string _authorId;
        private string _content;

        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public int RecipeId
        {
            get { return _recipeId; }
            set { _recipeId = value; }
        }

        public string AuthorId
        {
            get { return _authorId; }
            set { _authorId = value; }
        }

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
