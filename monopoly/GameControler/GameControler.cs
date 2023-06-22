using System;
using System.Collections.Generic;

namespace monopoly
{
    public class GameController
    {
        private IBoard _board;
        private List<IDice> _dices;
        private List<int> _results;
        private List<Player> _players;
        private Dictionary<Player, Square> _playerPositions;
        private Dictionary<Player, int> _playerMoney;
        private Dictionary<Player, List<Property>> _playerProperties;
        private Dictionary<Player, bool> _jailStatus;
        private int _currentPlayer;
        public event Action<Player> GoToJailEvent;

        public GameController(IBoard board)
        {
            _board = board;
            _players = new List<Player>();
            _dices = new List<IDice>();
            _playerMoney = new Dictionary<Player, int>();
            _playerPositions = new Dictionary<Player, Square>();
            _playerProperties = new Dictionary<Player, List<Property>>();
            _results = new List<int>();
            _currentPlayer = 0;
            _jailStatus = new Dictionary<Player, bool>();
        }

        public bool AddPlayer(string name)
        {

            Player player = new Player(name);
            _players.Add(player);
            _playerMoney[player] = 20000;
            _playerPositions[player] = _board.GetSquare(0);
            _jailStatus[player] = false;
            return true;
        }

        public void AddDice(int x)
        {
            Dice dice = new Dice(x);
            _dices.Add(dice);
        }

        public void Roll()
        {
            _results.Clear();
            int result = 0;
            foreach (var dice in _dices)
            {
                result = dice.Roll();
                _results.Add(result);
            }
        }

        public int TotalResult()
        {
            int total = 0;
            foreach (int result in _results)
            {
                total += result;
            }
            return total;
        }

        public List<int> GetDiceResults()
        {
            return _results;
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
                HandleSquareAction(newSquare);
                HandleBankruptPlayer(GetActivePlayer());
            }
        }

        public string GetSquareName()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();

            if (activePlayer != null && currentPosition >= 0)
            {
                Square currentSquare = _board.GetSquare(currentPosition);
                return currentSquare.GetName();
            }

            return null;
        }

        public string GetSquareDetails()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();

            if (activePlayer != null && currentPosition >= 0)
            {
                Square currentSquare = _board.GetSquare(currentPosition);
                return currentSquare.GetDescription();
            }

            return null;
        }

        public List<Property> GetPlayerProperties()
        {
            Player activePlayer = GetActivePlayer();
            if (activePlayer != null && _playerProperties.ContainsKey(activePlayer))
            {
                return _playerProperties[activePlayer];
            }
            return new List<Property>();
        }



        public bool BuyProperty()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();

            if (activePlayer != null && currentPosition >= 0)
            {
                Square currentSquare = _board.GetSquare(currentPosition);

                if (!(currentSquare is Property property) || property.GetOwner() != null)
                {
                    return false;
                }

                int startPosition = _board.GetSquare(0).GetPosition();
                int numSquares = _board.GetSquaresCount();

                int totalSteps = TotalResult();
                int stepsPassed = (currentPosition - startPosition + totalSteps) % numSquares;
                if (stepsPassed <= totalSteps)
                {
                    return false;
                }

                int propertyPrice = property.GetPrice();

                if (_playerMoney.ContainsKey(activePlayer) && _playerMoney[activePlayer] >= propertyPrice)
                {
                    _playerMoney[activePlayer] -= propertyPrice;
                    _playerPositions[activePlayer] = property;
                    property.SetOwner(activePlayer.GetName());
                    if (!_playerProperties.ContainsKey(activePlayer))
                    {
                        _playerProperties[activePlayer] = new List<Property>();
                    }
                    _playerProperties[activePlayer].Add(property);

                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }


        public void HandleSquareAction(Square square)
        {
            Player activePlayer = GetActivePlayer();

            switch (square)
            {
                case Property property:
                    HandlePropertyAction(property);
                    break;
                case Tax tax:
                    PayTax(tax);
                    break;
                case Card card:
                    break;
                case GoToJail goToJail:
                    MoveToJail();
                    break;
                default:
                    break;
            }
        }

        public void PayTax(Tax tax)
        {
            Player activePlayer = GetActivePlayer();
            int taxAmount = tax.GetTaxAmount();

            if (_playerMoney.ContainsKey(activePlayer))
            {
                _playerMoney[activePlayer] -= taxAmount;
            }
        }

        public void HandlePropertyAction(Property property)
        {
            Player activePlayer = GetActivePlayer();
            string propertyOwnerName = property.GetOwner();
            string currentPlayerName = activePlayer.GetName();

            if (propertyOwnerName != null && propertyOwnerName != currentPlayerName)
            {

                int rentAmount = property.GetRent();

                if (_playerMoney.ContainsKey(activePlayer))
                {
                    _playerMoney[activePlayer] -= rentAmount;
                }

                Player propertyOwner = GetPlayerByName(propertyOwnerName);

                if (_playerMoney.ContainsKey(propertyOwner))
                {
                    _playerMoney[propertyOwner] += rentAmount;
                }
            }
        }


        public Player GetPlayerByName(string playerName)
        {
            foreach (Player player in _players)
            {
                if (player.GetName() == playerName)
                {
                    return player;
                }
            }

            return null;
        }

        public Jail GetJailSquare()
        {
            for (int position = 0; position < _board.GetSquaresCount(); position++)
            {
                Square square = _board.GetSquare(position);
                if (square is Jail jailSquare)
                {
                    return jailSquare;
                }
            }
            return null;
        }

        public void MoveToJail()
        {
            Player activePlayer = GetActivePlayer();
            Jail jailSquare = GetJailSquare();

            if (activePlayer != null && jailSquare != null)
            {
                SetPlayerPosition(jailSquare);
                _jailStatus[activePlayer] = true;
                GoToJailEvent?.Invoke(activePlayer);
            }
        }

        public bool IsCurrentPlayerBankrupt()
        {
            Player activePlayer = GetActivePlayer();

            if (activePlayer != null)
            {
                if (_playerMoney.ContainsKey(activePlayer) && _playerMoney[activePlayer] <= 0)
                {
                    return true;
                }
                if (_playerProperties.ContainsKey(activePlayer) && _playerProperties[activePlayer].Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void EndGame()
        {
            if (_players.Count == 1)
            {
                Player winner = _players[0];

                foreach (var player in _players)
                {
                    if (_playerMoney[player] > _playerMoney[winner])
                    {
                        winner = player;
                    }
                    else if (_playerMoney[player] == _playerMoney[winner])
                    {
                        if (_playerProperties[player].Count > _playerProperties[winner].Count)
                        {
                            winner = player;
                        }
                    }
                }
            }
        }

        public void HandleBankruptPlayer(Player player)
        {
            if (IsCurrentPlayerBankrupt())
            {
                EndGame();
            }
        }

























    }
}
