using CampusLove.Application.UI;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var userAppMenu = new AppUserMenu();
        userAppMenu.ShowMenu();
    }
}