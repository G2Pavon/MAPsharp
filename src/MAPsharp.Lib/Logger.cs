namespace MAPsharp.Lib;

public static class Logger
{
    public static void Header(string version)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==========================================");
        Console.WriteLine($"   MAPsharp v{version}");
        Console.WriteLine("==========================================");
        Console.ResetColor();
    }

    public static void Step(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] {message}");
        Console.ResetColor();
    }

    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"   {message}");
        Console.ResetColor();
    }
    public static void Msg(string message) 
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void Success(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"   {message}");
        Console.ResetColor();
    }
    
    public static void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARNING] {message}");
        Console.ResetColor();
    }

    public static void Error(string message, Exception? ex = null)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[ERROR]");
        Console.WriteLine(message);
        
        if (ex != null)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Details: {ex.Message}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Stack Trace:");
            Console.WriteLine(ex.StackTrace);
        }
        
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void Footer(string outputPath)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("MAP CONVERTED SUCCESSFULLY");
        Console.WriteLine("------------------------------");
        Console.ResetColor();
        Console.WriteLine();
    }
}