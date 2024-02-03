namespace CrockPot.Models
{
    public class SharedRecipe
    {
        private int _id;
        private string _senderId;
        private string _receiverId;
        private int _recipeId;
        private DateTime _timestamp;

        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public string SenderId
        {
            get { return _senderId; }
            set { _senderId = value; }
        }

        public string ReceiverId
        {
            get { return _receiverId; }
            set { _receiverId = value; }
        }

        public int RecipeId
        {
            get { return _recipeId; }
            set { _recipeId = value; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }
    }
}
