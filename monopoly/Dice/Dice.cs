using System;

namespace monopoly 
{
    public class Dice : IDice 
    {
        private int _sideDice;

        public Dice(int sideDice) 
        {
           _sideDice = sideDice;
        }

        public int Roll() 
        {
            Random _random = new Random();
            return _random.Next(1, _sideDice + 1);
        }
    }
}
