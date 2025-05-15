using CampusLove.Application.UI;
using CampusLove.ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var mainMenu = new MainMenu();
        mainMenu.ShowMenu();
    }
}