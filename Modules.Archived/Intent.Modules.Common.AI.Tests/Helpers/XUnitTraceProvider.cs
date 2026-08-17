using System;
using Intent.Engine;
using Xunit.Abstractions;

namespace Intent.Modules.Common.AI.Tests.Helpers;

internal class XUnitTraceProvider : ITracing
{
    private readonly ITestOutputHelper _output;

    public XUnitTraceProvider(ITestOutputHelper output)
    {
        _output = output;
    }

    public void Debug(string message)
    {
        _output.WriteLine($"DEBUG: {message}");
    }

    public void Failure(Exception exception)
    {
        _output.WriteLine($"FAILURE: {exception.Message}");
    }

    public void Failure(string exceptionMessage)
    {
        _output.WriteLine($"FAILURE: {exceptionMessage}");
    }

    public void Info(string message)
    {
        _output.WriteLine($"INFO: {message}");
    }

    public void Warning(string message)
    {
        _output.WriteLine($"WARNING: {message}");
    }
}