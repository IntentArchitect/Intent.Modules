using System;
using System.Collections.Generic;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Tests.NuGet.Helpers
{
    internal class TestTracing : ITracing
    {
        public List<string> DebugEntries { get; } = new List<string>();
        public List<Exception> FailureEntriesOfTypeException { get; } = new List<Exception>();
        public List<string> FailureEntriesOfTypeString { get; } = new List<string>();
        public List<string> InfoEntries { get; } = new List<string>();
        public List<string> WarningEntries { get; } = new List<string>();
        public void Debug(string message) => DebugEntries.Add(message);
        public void Failure(Exception exception) => FailureEntriesOfTypeException.Add(exception);
        public void Failure(string exceptionMessage) => FailureEntriesOfTypeString.Add(exceptionMessage);
        public void Info(string message) => InfoEntries.Add(message);
        public void Warning(string message) => WarningEntries.Add(message);
    }
}