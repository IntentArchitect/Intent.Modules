using Intent.Modules.Common.DriftReporting.Api;

namespace Intent.Modules.Common.DriftReporting.Events
{
    /// <summary>
    /// A reporter registering with the run's drift checks. The run's event dispatcher casts the payload to this type, so if
    /// a second copy of this assembly were loaded the cast would throw rather than the reporter silently not running.
    /// </summary>
    internal sealed class DriftReporterRegistered
    {
        public const string EventId = "Intent.Common.DriftReporting.ReporterRegistered";

        public DriftReporterRegistered(DriftReporterBase reporter)
        {
            Reporter = reporter;
        }

        public DriftReporterBase Reporter { get; }
    }
}
