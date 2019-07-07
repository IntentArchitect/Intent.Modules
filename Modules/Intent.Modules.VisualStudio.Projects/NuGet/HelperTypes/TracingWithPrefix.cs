using System;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes
{
    internal class TracingWithPrefix : ITracing
    {
        private readonly ITracing _tracing;
        private readonly string _prefix;

        public TracingWithPrefix(ITracing tracing, string prefix)
        {
            _tracing = tracing;
            _prefix = prefix;
        }

        public void Debug(string message) => _tracing.Debug($"{_prefix}{message}");

        public void Failure(Exception exception) => _tracing.Failure(exception);

        public void Failure(string exceptionMessage) => _tracing.Failure($"{_prefix}{exceptionMessage}");

        public void Info(string message) => _tracing.Info($"{_prefix}{message}");

        public void Warning(string message) => _tracing.Warning($"{_prefix}{message}");
    }
}