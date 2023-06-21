namespace monopoly
{
    public class Jail : Square
    {
        private int _bailAmount;

        public Jail(int position, string name, string description, int bailAmount)
            : base(position, name, description)
        {
            _bailAmount = bailAmount;
        }

        public int GetBailAmount()
        {
            return _bailAmount;
        }
    }
}
