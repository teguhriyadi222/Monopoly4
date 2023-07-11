namespace monopoly.test;

public class UnitTest1
{
    [Fact]
    public void Dice()
    {
        
        Dice dice = new Dice(6);
        int result = dice.Roll();

        int expected = 5;

        Assert.Equal(expected, result);

    }

    [Fact]
    public void AddPlayerSucces()
    {
        Board board = new Board();
        GameController gameController = new GameController(board);
        bool result = gameController.AddPlayer("teguh");
        Assert.True(result);
       
    }
    [Fact]
        public void CurrentPlayerIndex()
        {

            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("Player1");
            gameController.AddPlayer("Player2");
            int currentPlayerIndexBefore = gameController.GetCurrentPlayerIndex();
            gameController.NextTurn();
            int currentPlayerIndexAfter = gameController.GetCurrentPlayerIndex();
            Assert.NotEqual(currentPlayerIndexBefore, currentPlayerIndexAfter);
           
        }
    [Fact]
        public void PlayerMoney()
        {
            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("teguh");

            int money = gameController.GetPlayerMoney();
            
            AssemblyLoadEventArgs.Equals(20000, money);
           
        }
    [Fact]
        public void Move_PlayerMovesToNewPosition()
        {
        
            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("Player1");
            gameController.AddDice(6);
            gameController.Roll();
            int currentPosition = gameController.GetPlayerPosition();

          
            gameController.Move();
            int newPosition = gameController.GetPlayerPosition();

            
            Assert.NotEqual(currentPosition, newPosition);
        }

        [Fact]
        public void BuyProperty_PropertyOwned_ReturnsPropertyOwnedError()
        {
            
            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("Player1");
            gameController.AddPlayer("Player2");
            gameController.AddDice(6);
            gameController.Roll();
            gameController.Move();
            gameController.NextTurn();
            gameController.Roll();
            gameController.Move();
            int currentPosition = gameController.GetPlayerPosition();

            
            BuyPropertyError result = gameController.BuyProperty();

           
            Assert.Equal(BuyPropertyError.PropertyOwned, result);
        }

        [Fact]
        public void SellProperty_PropertyOwnedAndValid_ReturnsTrue()
        {
           
            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("Player1");
            gameController.AddDice(6);
            gameController.Roll();
            gameController.Move();
            int currentPosition = gameController.GetPlayerPosition();
            gameController.BuyProperty();

           
            bool result = gameController.SellProperty();

            Assert.True(result);
        }

        [Fact]
        public void BuyHouse_PropertyOwnedAndValid_ReturnsTrue()
        {
       
            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("Player1");
            gameController.AddDice(6);
            gameController.Roll();
            gameController.Move();
            int currentPosition = gameController.GetPlayerPosition();
            gameController.BuyProperty();

           
            bool result = gameController.BuyHouse();

            Assert.True(result);
        }

        [Fact]
        public void BuyHotel_PropertyOwnedAndValid_ReturnsTrue()
        {

            IBoard board = new Board();
            GameController gameController = new GameController(board);
            gameController.AddPlayer("Player1");
            gameController.AddDice(6);
            gameController.Roll();
            gameController.Move();
            int currentPosition = gameController.GetPlayerPosition();
            gameController.BuyProperty();


            bool result = gameController.BuyHotel();

            Assert.True(result);
        }
        
}