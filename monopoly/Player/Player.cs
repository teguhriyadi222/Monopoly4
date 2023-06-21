using System;

namespace monopoly
{
    public interface IPlayer
    {
        int GetID();
        string GetName();
    }

    public class Player : IPlayer
    {
        private int _id;
        private string _name;
        private static int nextID = 1;

        public Player(string name)
        {
            _id = nextID++;
            _name = name;
        }

        public int GetID()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }
    }
}
