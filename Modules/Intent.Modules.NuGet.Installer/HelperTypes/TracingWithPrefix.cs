using System;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.NuGet.Installer.HelperTypes
{
    internal class TracingWithPrefix : ITracing
    {
        private readonly ITracing _tracing;
        private const string TracingOutputPrefix = "NuGet - ";

        public TracingWithPrefix(ITracing tracing) => _tracing = tracing;

        public void Debug(string message) => _tracing.Debug($"{TracingOutputPrefix}{message}");

        public void Failure(Exception exception) => _tracing.Failure(exception);

        public void Failure(string exceptionMessage) => _tracing.Failure($"{TracingOutputPrefix}{exceptionMessage}");

        public void Info(string message) => _tracing.Info($"{TracingOutputPrefix}{message}");

        public void Warning(string message) => _tracing.Warning($"{TracingOutputPrefix}{message}");
    }
}