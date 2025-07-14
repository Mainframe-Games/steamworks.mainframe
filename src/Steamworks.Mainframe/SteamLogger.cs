namespace Steamworks.Mainframe;

public static class SteamLogger
{
    public static void Debug(string message)
    {
        Print(message, Console.ForegroundColor);
    }

    public static void Warning(string message)
    {
        Print(message, ConsoleColor.Yellow);
    }
    
    public static void Error(string message)
    {
        Print(message, ConsoleColor.Red);
    }

    private static void Print(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }
}