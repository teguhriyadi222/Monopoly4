

namespace monopoly;

public delegate void MenuActionDelegate();
public class MenuOption
{
    public string Title { get; set; }
    public MenuActionDelegate Action { get; set; }
}

public class Game
{
    private bool turnEnd;
    // private bool gameEnd;
    private GameController gameController;
    private Player activePlayer;
    private List<MenuOption> menuOptions;


    public Game()
    {
        IBoard board = new Board();
        board.AddSquare(new Start(0, "Start", "Starting point of the board"));
        board.AddSquare(new Property(1, "Salatiga", "Description 1", 100, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(2, "Bandung", "Description 2", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(3, "Semarang", "Description 3", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(4, "jakarta", "Description 4", 300, 50, 30, 20, TypeProperty.Train));
        board.AddSquare(new Property(5, "Malang", "Description 5", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(6, "Purworejo", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Tax(7, "Tax 1", "kamu harus membayar: Rp.100", 100));
        board.AddSquare(new Property(8, "Yogyakarta", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(9, "Bekasi", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Jail(10, "Jail", "Jail square", 70));
        board.AddSquare(new Property(12, "Jambi", "Description 6", 300, 50, 30, 20, TypeProperty.Utility));
        board.AddSquare(new Tax(13, "Tax 2", "kamu harus membayar: Rp.200", 200));
        board.AddSquare(new Property(14, "Medan", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(15, "Surabaya", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(16, "Denpasar", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(17, "Ketapang", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(18, "Cilacap", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(19, "Cikarang", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(20, "Banten", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new FreeParking(21, "Free Parking", "Description 6"));
        board.AddSquare(new Tax(23, "Tax 3", "kamu harus membayar: Rp.300", 300));
        board.AddSquare(new Property(24, "JayaPura", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(25, "Palu", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(26, "Kutai", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(27, "Magelang", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(28, "Ambarawa", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new Property(29, "Ungaran", "Description 6", 300, 50, 30, 20, TypeProperty.Residential));
        board.AddSquare(new GoToJail(31, "GoToJail", "Description"));
        board.AddSquare(new GoToJail(32, "GoToJail", "Description"));
        board.AddSquare(new GoToJail(33, "GoToJail", "Description"));
        board.AddSquare(new GoToJail(34, "GoToJail", "Description"));
        board.AddSquare(new GoToJail(35, "GoToJail", "Description"));
        board.AddSquare(new GoToJail(36, "GoToJail", "Description"));
        board.AddSquare(new GoToJail(37, "GoToJail", "Description"));

        gameController = new GameController(board);
        activePlayer = gameController.GetActivePlayer();
        gameController.AddDice(6);
        gameController.AddDice(6);
        turnEnd = false;
        // gameEnd = false;

    }

    public async Task StartGame()
    {
        Console.Clear();
        Console.WriteLine("=================WELCOME TO MOOPOLY GAME=======================");
        Console.Write("Enter the number of players: ");

        int numberOfPlayers = 0;
        bool inputValid = false;
        while (!inputValid)
        {
            if (int.TryParse(Console.ReadLine(), out numberOfPlayers) && numberOfPlayers > 0)
            {
                inputValid = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number of players.");
                Console.Write("Enter the number of players: ");
            }
        }
        for (int i = 0; i < numberOfPlayers; i++)
        {
            Console.Write("Enter the name of player {0}: ", i + 1);
            string playerName = Console.ReadLine();

            if (!string.IsNullOrEmpty(playerName))
            {
                gameController.AddPlayer(playerName);
            }
            else
            {
                Console.WriteLine("Invalid input");
                i--;
            }
        }

        Console.WriteLine("Active Player : " + gameController.GetPlayerMoney());

        Console.WriteLine("Press enter to start the game");
        menuOptions = GetMenuOptions();
        Console.Clear();
        while (gameController.GetGameStatus() != true)
        {
            Player activePlayer = gameController.GetActivePlayer();
            Console.WriteLine("Player's Turn: " + activePlayer.GetName());
            Console.WriteLine("Press enter to Roll the Dice");
            Console.ReadKey();
            gameController.Roll();
            List<int> diceResults = gameController.GetDiceResults();
            int total = gameController.TotalResult();
            for (int i = 0; i < diceResults.Count; i++)
            {
                Console.WriteLine("Dice {0}: {1}", i + 1, diceResults[i]);
                await Task.Delay(2000);
            }

            Console.WriteLine("Total Dice : " + total);
            gameController.Move();
            Console.ReadKey();

            while (diceResults[0] == diceResults[1])
            {
                Console.WriteLine("Congratulations! You rolled doubles. Rolling again...");
                Console.WriteLine("Press enter to Roll the Dice");
                Console.ReadKey();
                gameController.Roll();
                diceResults = gameController.GetDiceResults();
                total = gameController.TotalResult();
                for (int i = 0; i < diceResults.Count; i++)
                {
                    Console.WriteLine("Dice {0}: {1}", i + 1, diceResults[i]);
                    await Task.Delay(2000);
                }

                Console.WriteLine("Total Dice : " + total);
                gameController.Move();
                Console.ReadKey();
            }
            Console.Clear();
            Console.WriteLine("your position :" + gameController.GetPlayerPosition());
            Console.WriteLine("name : " + gameController.GetSquareName());
            Console.ReadKey();

            while (!turnEnd)
            {

                ShowMenuOptions();

                int selectedOption = 0;
                bool isValidOption = false;
                while (!isValidOption)
                {
                    Console.Write("Select an option: ");
                    if (int.TryParse(Console.ReadLine(), out selectedOption) && selectedOption >= 1 && selectedOption <= menuOptions.Count)
                    {
                        isValidOption = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid option number.");
                    }
                }

                MenuOption selectedMenuOption = menuOptions[selectedOption - 1];
                selectedMenuOption.Action.Invoke();
            }
        }


    }
    private List<MenuOption> GetMenuOptions()
    {
        List<MenuOption> menuOptions = new List<MenuOption>
        {
            new MenuOption { Title = "Finish Turn", Action = FinishTurn },
            new MenuOption { Title = "Your DashBoard", Action = ShowDashboard },
            new MenuOption { Title = "Purchase the Property", Action = PurchaseProperty },
            new MenuOption { Title = "Quit Game", Action = QuitGame }
        };
        return menuOptions;
    }

    private void ShowMenuOptions()
    {
        Console.WriteLine("\n===== MENU OPTIONS =====");
        for (int i = 0; i < menuOptions.Count; i++)
        {
            Console.WriteLine("{0}. {1}", i + 1, menuOptions[i].Title);
        }
        Console.WriteLine("========================\n");
    }


    private void FinishTurn()
    {
        turnEnd = true;
        gameController.NextTurn();
    }

    private void ShowDashboard()
    {
        Console.Clear();
        List<Property> playerProperties = gameController.GetPlayerProperties();
        Console.WriteLine("your position :" + gameController.GetPlayerPosition());
        Console.WriteLine("your Money :" + gameController.GetPlayerMoney());
        Console.WriteLine("Your properties are: ");
        if (playerProperties.Count > 0)
        {
            foreach (var property in playerProperties)
            {

                Console.WriteLine($"- {property.GetName()}");

            }
        }
        else
        {
            Console.WriteLine("You don't have any properties.");
        }
    Console.ReadKey();
    }

    private void PurchaseProperty()
    {
        BuyPropertyError error = gameController.BuyProperty();
        switch(error)
        {
            case BuyPropertyError.InsufficientFunds :
            break;
            case BuyPropertyError.PropertyOwned :
            break;
            case BuyPropertyError.Succes :
            Console.WriteLine("Property successfully purchased!");
            break;
        }
    }

    private void BuyHouse()
    {
        bool buyHouse = gameController.BuyHouse();
        if (buyHouse)
        {
            Console.WriteLine("House successfully purchased");
        }
        else
        {
            Console.WriteLine("Failed to purchase house");
        }
    }

    private void BuyHotel()
    {
        bool buyHotel = gameController.BuyHotel();
        if(buyHotel)
        {
             Console.WriteLine("Hotel successfully purchased");

        }
        else
        {
            Console.WriteLine("Failed to purchase Hotel");
        }
    }

    private void SellProperty()
    {
        bool sellProperty = gameController.SellProperty();

        if (sellProperty)
        {
             Console.WriteLine("successful sale of property");
        }
        else
        {
             Console.WriteLine("failed sale of property");
        }
    }


    private void QuitGame()
    {
        gameController.SetGameStatus(true);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.StartGame().Wait();
        Console.ReadLine();
    }

}



