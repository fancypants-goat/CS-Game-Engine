namespace Engine;

public static class Debug
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Fatal,
        Exit,
        Launch
    }
    public static void LogAs(LogType type, params object[] messages)
    {
        string typeStr = "";

        switch (type)
        {
            case LogType.Info:
                typeStr = "INFO";
                break;
            case LogType.Warning:
                typeStr = "WARN";
                break;
            case LogType.Error:
                typeStr = "ERROR";
                break;
            case LogType.Fatal:
                typeStr = "FATAL";
                break;
            case LogType.Exit:
                typeStr = "EXIT";
                break;
            case LogType.Launch:
                typeStr = "START";
                break;
            defualt:
                typeStr = "UKNWN";
                break;
        }
        
        string timeString = DateTime.Now.ToString("HH:mm:ss");
        
        Console.Write("[ " + typeStr + " ] " + "[ " + timeString + " ]" + "  >> ");

        foreach (var msg in messages)
        {
            Console.Write(msg + " ");
        }
        
        Console.WriteLine();
    }
}