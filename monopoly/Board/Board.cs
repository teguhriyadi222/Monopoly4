using System.Collections.Generic;

namespace monopoly
{
    public interface IBoard
    {
        void AddSquare(Square square);
        Square GetSquare(int position);
        int GetSquaresCount();
    }

    public class Board : IBoard
    {
        private List<Square> _squares;

        public Board()
        {
            _squares = new List<Square>();
        }

        public void AddSquare(Square square)
        {
            _squares.Add(square);
        }

        public Square GetSquare(int position)
        {
            foreach (Square square in _squares)
            {
                if (square.GetPosition() == position)
                {
                    return square;
                }
            }
            return null;
        }
        public int GetSquaresCount()
        {
            return _squares.Count;
        }
    }
}
