using System;
using Intent.Utils;
using Microsoft.Extensions.Logging;

namespace Intent.Modules.Common.AI;

public class SoftwareFactoryLogger : ILogger
{
    private readonly string _categoryName;

    public SoftwareFactoryLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                Logging.Log.Debug($"{_categoryName}: {formatter(state, exception)}");
                break;
            case LogLevel.Information:
                Logging.Log.Info($"{_categoryName}: {formatter(state, exception)}");
                break;
            case LogLevel.Warning:
                Logging.Log.Warning($"{_categoryName}: {formatter(state, exception)}");
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                Logging.Log.Failure($"{_categoryName}: {formatter(state, exception)}");
                break;
            case LogLevel.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}