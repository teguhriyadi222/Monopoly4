namespace monopoly
{
    public abstract class Square
    {
        private int _position;
        private string _name;
        private string _description;

        public Square(int position, string name, string description)
        {
            _position = position;
            _name = name;
            _description = description;
        }

        public int GetPosition()
        {
            return _position;
        }

        public string GetName()
        {
            return _name;
        }

        public string GetDescription()
        {
            return _description;
        }
    }
}
