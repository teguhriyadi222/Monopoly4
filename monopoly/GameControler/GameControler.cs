using System;
using System.Collections.Generic;

namespace monopoly
{
    public class GameController
    {
        private IBoard _board;
        private List<IDice> _dices;
        private List<Player> _players;
        private Dictionary<Player, Square> _playerPositions;
        private Dictionary<Player, int> _playerMoney;
        private int _currentPlayer;

        public GameController(IBoard board)
        {
            _board = board;
            _players = new List<Player>();
            _dices = new List<IDice>();
            _playerMoney = new Dictionary<Player, int>();
            _playerPositions = new Dictionary<Player, Square>();
            _currentPlayer = 0;
        }

        public bool AddPlayer(string name)
        {
            Player player = new Player(name);
            _players.Add(player);
            _playerMoney[player] = 20000;
            return true;
        }

        public void AddDice(int x)
        {
            Dice dice = new Dice(x);
            _dices.Add(dice);
        }

        public List<int> Roll()
        {
            List<int> results = new List<int>();
            foreach (var dice in _dices)
            {
                int result = dice.Roll();
                results.Add(result);
            }
            return results;
        }

        public int TotalResult()
        {
            List<int> resultsDice = Roll();
            int total = 0;
            foreach (int result in resultsDice)
            {
                total += result;
            }
            return total;
        }
        public int GetCurrentPlayerIndex()
        {
            if (_currentPlayer < 0 || _currentPlayer >= _players.Count)
            {
                _currentPlayer = 0;
            }
            return _currentPlayer;
        }
        public void NextTurn()
        {
            _currentPlayer = (_currentPlayer + 1) % _players.Count;
        }

        public Player GetActivePlayer()
        {
            if (_players.Count > 0)
            {
                int currentPlayerIndex = GetCurrentPlayerIndex();
                Player activePlayer = _players[currentPlayerIndex];
                return activePlayer;
            }
            return null;
        }

        public int GetPlayerPosition()
        {
            Player activePlayer = GetActivePlayer();
            if (activePlayer != null && _playerPositions.ContainsKey(activePlayer))
            {
                Square square = _playerPositions[activePlayer];
                return square.GetPosition();
            }

            return -1;
        }

        public int GetPlayerMoney()
        {
            Player activePlayer = GetActivePlayer();
            if (_playerMoney.ContainsKey(activePlayer))
            {
                return _playerMoney[activePlayer];
            }
            return 0;
        }
        public void SetPlayerPosition(Square square)
        {
            Player activePlayer = GetActivePlayer();
            if (_playerPositions.ContainsKey(activePlayer))
            {
                _playerPositions[activePlayer] = square;
            }
            else
            {
                _playerPositions.Add(activePlayer, square);
            }
        }

        public void Move()
        {
            int steps = TotalResult();
            int currentPosition = GetPlayerPosition();
            if (currentPosition >= 0)
            {
                int numSquares = _board.GetSquaresCount();
                int newPosition = (currentPosition + steps) % numSquares;

                Square newSquare = _board.GetSquare(newPosition);
                SetPlayerPosition(newSquare); 
            }
        }














    }
}
