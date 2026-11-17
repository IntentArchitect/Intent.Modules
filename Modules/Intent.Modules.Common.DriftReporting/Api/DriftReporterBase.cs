using Intent.Engine;
using Intent.Modules.Common.DriftReporting.Events;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.DriftReporting.Api
{
    /// <summary>
    /// A drift reporter: a factory extension that registers itself with the run's drift checks, which call
    /// <see cref="Report"/> once every template has run, in <see cref="FactoryExtensionBase.Order"/>. The reporter keeps its
    /// own detection; it only adds its findings to <see cref="IDriftContext.Area"/>s, and the Intent.Common.DriftReporting
    /// module logs them and writes the application's one drift report.
    /// </summary>
    public abstract class DriftReporterBase : FactoryExtensionBase
    {
        /// <summary>
        /// Checks the application and adds what it finds to the context's Areas. Called after template execution, when
        /// every template's output has been built but nothing is staged yet. A reporter that throws is logged, and the
        /// other reporters still run.
        /// </summary>
        protected abstract void Report(IDriftContext context);

        /// <summary>
        /// Publishes the reporter's registration on the run's event dispatcher. The drift checks subscribe at the run's
        /// start, before any extension's registration step, so the order of the extensions doesn't matter.
        /// </summary>
        protected sealed override void OnBeforeTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Publish(DriftReporterRegistered.EventId, new DriftReporterRegistered(this));
        }

        internal void Run(IDriftContext context) => Report(context);
    }
}
