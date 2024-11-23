namespace Approvers.King.Common;

public static class LogManager
{
    private static string CreateTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static void Log(string message)
    {
        Console.WriteLine($"{CreateTimestamp()}: [INFO] {message}");
    }

    public static void LogError(string message)
    {
        Console.Error.WriteLine($"{CreateTimestamp()}: [ERROR] {message}");
    }

    public static void LogException(Exception exception)
    {
        Console.Error.WriteLine($"{CreateTimestamp()}: [EXCEPTION] {exception.Message}");
        Console.Error.Write(exception);
    }
}
