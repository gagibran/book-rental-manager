namespace BookRentalManager.Application.Extensions;

public static class ILoggerExtensions
{
    public static void LogIfLevelEnabled<TArg1>(this ILogger logger, LogLevel logLevel, string message, TArg1 arg1)
    {
        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, message, arg1);
        }
    }

    public static void LogIfLevelEnabled<TArg1, TArg2>(
        this ILogger logger,
        LogLevel logLevel,
        string message,
        TArg1 arg1,
        TArg2 arg2)
    {
        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, message, arg1, arg2);
        }
    }

    public static void LogIfLevelEnabled<TArg1, TArg2, TArg3>(
        this ILogger logger,
        LogLevel logLevel,
        string message,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3)
    {
        if (logger.IsEnabled(logLevel))
        {
            logger.Log(logLevel, message, arg1, arg2, arg3);
        }
    }
}
