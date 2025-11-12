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
        Launch,
        Debug
    }

    public static bool PRINT_LOG_WARNINGS = true;

    public static void LogPrefixed(string type, params object[] messages)
    {
        if (type.Length > 5 && PRINT_LOG_WARNINGS)
            LogPrefixed("DEV",
                "For readability, keep [type] of LogPrefixed(type, messages) shorter than 6 characters.");

        var timeString = DateTime.Now.ToString("HH:mm:ss");

        Console.Write($"[ {type} ]".PadRight(12) + $"[ {timeString} ]  >> ");

        foreach (var msg in messages) Console.Write(msg + " ");

        Console.WriteLine();
    }

    public static void LogPrefixed(LogType type, params object[] messages)
    {
        var typeStr = type switch
        {
            LogType.Info => "INFO",
            LogType.Warning => "WARN",
            LogType.Error => "ERROR",
            LogType.Fatal => "FATAL",
            LogType.Launch => "START",
            LogType.Exit => "EXIT",
            LogType.Debug => "DEBUG",
            _ => "UNKWN"
        };

        LogPrefixed(typeStr, messages);
    }

    public static void LogInfo(params object[] messages)
    {
        LogPrefixed(LogType.Info, messages);
    }

    public static void Log(params object[] messages)
    {
        LogPrefixed(LogType.Debug, messages);
    }

    public static void LogWarn(params object[] messages)
    {
        LogPrefixed(LogType.Warning, messages);
    }

    public static void LogError(params object[] messages)
    {
        LogPrefixed(LogType.Error, messages);
    }

    public static void LogFatal(params object[] messages)
    {
        LogPrefixed(LogType.Fatal, messages);
    }

    public static void LogMemLeak(string name, params object[] messages)
    {
        LogPrefixed("LEAK", $"Memory leak detected in {name} instance! Did not call Dispose().\n\t", messages);
    }
}