using Microsoft.Extensions.Logging;

namespace Intent.Modules.Common.AI;

public class SoftwareFactoryLoggingProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new SoftwareFactoryLogger(categoryName);
    }

    public void Dispose()
    {
    }
}