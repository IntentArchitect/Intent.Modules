using Intent.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.RegularExpressions;

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
        var message = UnescapeAndPrettify(formatter(state, exception));

        switch (logLevel)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                Logging.Log.Debug($"{_categoryName}: {message}");
                break;
            case LogLevel.Information:
                Logging.Log.Info($"{_categoryName}: {message}");
                break;
            case LogLevel.Warning:
                Logging.Log.Warning($"{_categoryName}: {message}");
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                Logging.Log.Failure($"{_categoryName}: {message}");
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

    public static string UnescapeAndPrettify(string input)
    {
        input = Regex.Unescape(input);
        input = WebUtility.HtmlDecode(input);

        return input
            .Replace("\\r\\n", "\r\n")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\"", "\"")
            .Replace("\\\\", "\\");
    }
}