namespace monopoly
{
    public enum TypeProperty
    {
        Residential,
        Train,
        Utility
    }

    public enum PropertySituation
    {
        Unowned,
        Owned,
        Mortgaged
    }

    public class Property : Square
    {
        private string _owner;
        private int _price;
        private int _rent;
        private int _housePrice;
        private int _hotelPrice;
        private int _numberOfHouses;
        private bool _hasHotel;
        private TypeProperty _propertyType;
        private PropertySituation _propertySituation;

        public Property(int position, string name, string description, int price, int rent, int housePrice, int hotelPrice, TypeProperty propertyType)
            : base(position, name, description)
        {
            _price = price;
            _rent = rent;
            _housePrice = housePrice;
            _hotelPrice = hotelPrice;
            _propertyType = propertyType;
            _owner = null;
            _propertySituation = PropertySituation.Unowned;
            _numberOfHouses = 0;
            _hasHotel = false;
        }

        public string GetOwner()
        {
            return _owner;
        }

        public void SetOwner(string playerName)
        {
            _owner = playerName;
            _propertySituation = PropertySituation.Owned;
        }

        public int GetPrice()
        {
            return _price;
        }

        public int GetRent()
        {
            int totalRent = _rent;

            if (_numberOfHouses > 0)
            {
                totalRent = _rent + _housePrice * _numberOfHouses;
            }
            else if (_hasHotel)
            {
                totalRent = _rent + _housePrice;
            }

            return totalRent;
        }

        public TypeProperty GetPropertyType()
        {
            return _propertyType;
        }

        public PropertySituation GetPropertySituation()
        {
            return _propertySituation;
        }

        public void SetPropertySituation(PropertySituation situation)
        {
            _propertySituation = situation;
        }

        public void AddHouse()
        {
            _numberOfHouses++;
        }

        public void RemoveHouse()
        {
            _numberOfHouses--;
        }

        public void AddHotel()
        {
            _hasHotel = true;
        }

        public void RemoveHotel()
        {
            _hasHotel = false;
        }

        public int GetNumberOfHouses()
        {
            return _numberOfHouses;
        }

        public bool HasHotel()
        {
            return _hasHotel;
        }

        public int GetHousePrice()
        {
            return _housePrice;
        }

        public int GetHotelPrice()
        {
            return _hotelPrice;
        }
    }
}
