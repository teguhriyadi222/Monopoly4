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
        
}