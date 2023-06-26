using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;


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
        private Dictionary<Player, int> _jailTurns;
        private List<Card> _chanceCards;
        private List<Card> _communityChestCards;
        private int _currentPlayer;
        private bool _gameStatus;
        public event Action<Player> GoToJailEvent;
        public event Action<int> TaxNotificationEvent;
        private static readonly ILog log = LogManager.GetLogger(typeof(GameController));

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
            _gameStatus = false;
            _jailStatus = new Dictionary<Player, bool>();
            _jailTurns = new Dictionary<Player, int>();
            _chanceCards = new List<Card>();
            _communityChestCards = new List<Card>();
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        public bool AddPlayer(string name)
        {
            Player player = new Player(name);
            _players.Add(player);
            _playerMoney[player] = 20000;
            _playerPositions[player] = _board.GetSquare(0);
            _jailStatus[player] = false;
            _jailTurns[player] = 0;
            log.Info($"Adding player: {name}");
            return true;
        }

        public bool AddDice(int x)
        {
            Dice dice = new Dice(x);
            _dices.Add(dice);
            log.Info($"Adding dice with {x} sides");
            return true;
        }

        public bool Roll()
        {
            _results.Clear();
            int result = 0;
            foreach (var dice in _dices)
            {
                result = dice.Roll();
                _results.Add(result);
            }
            log.Info("Rolling the dice");
            return true;
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

        private bool HasRolledDoubles()
        {
            List<int> diceResults = GetDiceResults();
            return diceResults.Count >= 2 && diceResults[0] == diceResults[1];
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

        public int GetPlayerPropertyHouses()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();

            if (activePlayer != null && currentPosition >= 0)
            {
                Square currentSquare = _board.GetSquare(currentPosition);

                if (currentSquare is Property property && property.GetOwner() == activePlayer.GetName())
                {
                    return property.GetNumberOfHouses();
                }
            }

            return 0;
        }


        public BuyPropertyError BuyProperty()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();

            if (activePlayer != null && currentPosition >= 0)
            {
                Square currentSquare = _board.GetSquare(currentPosition);

                if (!(currentSquare is Property property) || property.GetOwner() != null)
                {
                    return BuyPropertyError.PropertyOwned;
                }

                int startPosition = _board.GetSquare(0).GetPosition();
                int numSquares = _board.GetSquaresCount();

                int totalSteps = TotalResult();
                int stepsPassed = (currentPosition - startPosition + totalSteps) % numSquares;
                if (stepsPassed <= totalSteps)
                {
                    return BuyPropertyError.PropertyOwned;
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

                }
                else
                {
                    return BuyPropertyError.InsufficientFunds;
                }
            }

            return BuyPropertyError.Succes;
        }

        public bool SellProperty()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();
            Square currentSquare = _board.GetSquare(currentPosition);
            Property property = currentSquare as Property;
            if (_playerProperties.ContainsKey(activePlayer))
            {
                List<Property> properties = _playerProperties[activePlayer];
                if (properties.Contains(property) && property.GetOwner() == activePlayer.GetName())
                {
                    int propertyPrice = property.GetPrice();

                    if (_playerMoney.ContainsKey(activePlayer))
                    {
                        _playerMoney[activePlayer] += propertyPrice;
                        properties.Remove(property);
                        property.SetOwner(null);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        public bool BuyHouse()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();
            Square currentSquare = _board.GetSquare(currentPosition);
            Property property = currentSquare as Property;

            if (property.GetPropertySituation() != PropertySituation.Owned || property.GetOwner() != activePlayer.GetName())
            {
                return false;
            }

            if (property.GetPropertyType() != TypeProperty.Residential)
            {
                return false;
            }

            int housePrice = property.GetHousePrice();
            int maxHouses = 4;

            if (property.GetNumberOfHouses() >= maxHouses)
            {
                return false;
            }

            if (_playerMoney.ContainsKey(activePlayer) && _playerMoney[activePlayer] >= housePrice)
            {
                _playerMoney[activePlayer] -= housePrice;
                property.AddHouse();
                return true;
            }
            else
            {
                return false;
            }


        }

        public bool BuyHotel()
        {
            Player activePlayer = GetActivePlayer();
            int currentPosition = GetPlayerPosition();
            Square currentSquare = _board.GetSquare(currentPosition);
            Property property = currentSquare as Property;

            if (property.GetPropertySituation() != PropertySituation.Owned || property.GetOwner() != activePlayer.GetName())
            {
                return false;
            }

            if (property.GetPropertyType() != TypeProperty.Residential)
            {
                return false;
            }

            int hotelPrice = property.GetHotelPrice();

            if (_playerMoney.ContainsKey(activePlayer) && _playerMoney[activePlayer] >= hotelPrice)
            {
                _playerMoney[activePlayer] -= hotelPrice;
                property.AddHotel();
                return true;
            }
            else
            {
                return false;
            }
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
            TaxNotificationEvent?.Invoke(taxAmount);
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
                _jailTurns[activePlayer] = 0;
                GoToJailEvent?.Invoke(activePlayer);
            }
        }

        private void IncrementJailTurns()
        {
            foreach (var player in _jailStatus.Keys.ToList())
            {
                if (_jailStatus[player])
                {
                    _jailTurns[player]++;
                }
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
                SetGameStatus(true);
            }
        }

        public bool GetGameStatus()
        {
            return _gameStatus;
        }

        public void SetGameStatus(bool status)
        {
            _gameStatus = status;
        }

        public bool GetOutOfJail()
        {
            Player activePlayer = GetActivePlayer();

            if (activePlayer != null && _jailStatus.ContainsKey(activePlayer) && _jailStatus[activePlayer])
            {
                List<int> diceResults = GetDiceResults();

                if (diceResults.Count == 2 && diceResults[0] == diceResults[1])
                {
                    _jailStatus[activePlayer] = false;
                    return true;
                }
            }

            return false;
        }

        public bool PayToGetOutOfJail()
        {
            Player activePlayer = GetActivePlayer();

            if (activePlayer != null && _jailStatus.ContainsKey(activePlayer) && _jailStatus[activePlayer])
            {
                Jail jailSquare = GetJailSquare();

                if (jailSquare != null)
                {
                    int bailAmount = jailSquare.GetBailAmount();

                    if (_playerMoney.ContainsKey(activePlayer) && _playerMoney[activePlayer] >= bailAmount)
                    {
                        _playerMoney[activePlayer] -= bailAmount;
                        _jailStatus[activePlayer] = false;
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetBailAmount()
        {
            Jail jailSquare = GetJailSquare();
            return jailSquare.GetBailAmount();

        }

        public List<Player> GetJailedPlayers()
        {
            List<Player> jailedPlayers = new List<Player>();

            foreach (var player in _jailStatus.Keys)
            {
                if (_jailStatus[player])
                {
                    jailedPlayers.Add(player);
                }
            }

            return jailedPlayers;
        }

        public void ExecuteCommand(Card card)
        {
            Player activePlayer = GetActivePlayer();
            TypeCardCommand typeCardCommand = card.GetTypeCommand();
            int valueCard = card.GetValue();
            switch (typeCardCommand)
            {
                case TypeCardCommand.Move:
                    Move();
                    break;
                case TypeCardCommand.PayTax:
                    _playerMoney[activePlayer] -= valueCard;
                    break;
                case TypeCardCommand.ReceiveMoney:
                    _playerMoney[activePlayer] += valueCard;
                    break;
            }

        }

        public void AddCard(Card card)
        {
            if (card.GetCardType() == TypeCard.Chance)
            {
                _chanceCards.Add(card);
            }
            else if (card.GetCardType() == TypeCard.CommunityChest)
            {
                _communityChestCards.Add(card);
            }
        }

        public Card GetRandomChanceCard()
        {
            Random random = new Random();
            int index = random.Next(_chanceCards.Count);
            Card randomCard = _chanceCards[index];
            return randomCard;
        }

        public Card GetRandomCommunityChestCard()
        {
            Random random = new Random();
            int index = random.Next(_communityChestCards.Count);
            Card randomCard = _communityChestCards[index];
            return randomCard;
        }

        public void ShuffleChanceCards()
        {
            Random random = new Random();
            int n = _chanceCards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card temp = _chanceCards[k];
                _chanceCards[k] = _chanceCards[n];
                _chanceCards[n] = temp;
            }
        }

        public void ShuffleCommunityChestCards()
        {
            Random random = new Random();
            int n = _communityChestCards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card temp = _communityChestCards[k];
                _communityChestCards[k] = _communityChestCards[n];
                _communityChestCards[n] = temp;
            }
        }




    }
}
