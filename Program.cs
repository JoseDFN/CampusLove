using System.Text;
using CampusLove.Application.UI;
using CampusLove.ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var mainMenu = new MainMenu();
        mainMenu.ShowMenu();
    }
}