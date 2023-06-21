
namespace monopoly
{

    public class Tax : Square
    {
        private int _taxAmount;

        public Tax(int position, string name, string description, int taxAmount)
            : base(position, name, description)
        {
            _taxAmount = taxAmount;
        }

        public int GetTaxAmount()
        {
            return _taxAmount;
        }
    }

}

