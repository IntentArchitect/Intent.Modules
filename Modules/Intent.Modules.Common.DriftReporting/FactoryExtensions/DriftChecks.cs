using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Intent.Engine;
using Intent.Eventing;
using Intent.Exceptions;
using Intent.Modules.Common;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.DriftReporting.Events;
using Intent.Modules.Common.DriftReporting.Reporting;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Common.DriftReporting.FactoryExtensions
{
    /// <summary>
    /// The run's drift checks. At the start of the run it subscribes to the reporters' registrations (the run's event
    /// dispatcher only reaches handlers already subscribed, and every extension's start comes before any extension's
    /// registration step). After template execution, when every template's output is built but nothing is staged, it runs
    /// each registered reporter in order, logs their findings, and stages one drift report when there are any.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class DriftChecks : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.DriftReporting.DriftChecks";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>Each run's reporters, by the run's event dispatcher, which is created for each run.</summary>
        [IntentManaged(Mode.Ignore)]
        private readonly ConditionalWeakTable<IApplicationEventDispatcher, DriftRun> _runs = new();

        /// <summary>The CLI's run-level parameter that is <c>true</c> in a CI/CD run (<c>--param ensure-no-outstanding-changes=true</c>).</summary>
        [IntentManaged(Mode.Ignore)]
        internal const string CiParameter = "ensure-no-outstanding-changes";

        [IntentManaged(Mode.Ignore)]
        protected override void OnStart(IApplication application) => Start(application, IsCi(application));

        [IntentManaged(Mode.Ignore)]
        protected override void OnAfterTemplateExecution(IApplication application) => Finish(application, IsCi(application));

        [IntentManaged(Mode.Ignore)]
        internal void Start(IApplication application, bool onCi)
        {
            var run = new DriftRun();
            _runs.AddOrUpdate(application.EventDispatcher, run);
            application.EventDispatcher.Subscribe<DriftReporterRegistered>(DriftReporterRegistered.EventId, e => run.Register(e.Reporter));

            // The last run's report is deleted, so a run with drift stages its report as a new file (the Changes panel hides a
            // change that matches the disk), and a clean run leaves none behind. Not in a CI/CD run, which never writes one.
            if (!onCi)
            {
                DeleteLastReport(application);
            }
        }

        /// <summary>
        /// Runs the reporters and logs their findings. Outside CI/CD it stages the report when there are any; in a CI/CD run,
        /// where a staged report would be an outstanding change, it writes none, and a drift error then ends the run.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        internal void Finish(IApplication application, bool onCi)
        {
            if (!_runs.TryGetValue(application.EventDispatcher, out var run))
            {
                return;
            }

            _runs.Remove(application.EventDispatcher);
            if (run.Reporters.Count == 0)
            {
                return;
            }

            var collector = Check(application, run.Reporters, Logging.Log);
            Publish(application, collector, Logging.Log, writesReport: !onCi);
            FailOnCi(collector, onCi);
        }

        /// <summary>
        /// Whether this is a CI/CD run (<see cref="CiParameter"/>). <c>GetParameter</c> arrived in SDK 3.16, after the one this
        /// module builds against, so it is found by reflection: an older host has no parameters, so its runs never are.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        internal static bool IsCi(ISoftwareFactoryExecutionContext context)
        {
            // TODO: when Intent.SoftwareFactory.SDK 3.16 is published, build against it and replace this reflection with
            // context.GetParameter(CiParameter).
            var getParameter = typeof(ISoftwareFactoryExecutionContext).GetMethod("GetParameter", new[] { typeof(string) });
            try
            {
                return string.Equals(getParameter?.Invoke(context, new object[] { CiParameter }) as string, "true", StringComparison.OrdinalIgnoreCase);
            }
            catch (TargetInvocationException)
            {
                return false;
            }
        }

        /// <summary>
        /// In a CI/CD run a drift error ends the run with an exception, once every finding is logged, so the pipeline breaks
        /// on drift. Warnings never do.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        internal static void FailOnCi(DriftCollector collector, bool onCi)
        {
            var areas = collector.Areas.Where(x => x.Errors > 0).ToList();
            if (!onCi || areas.Count == 0)
            {
                return;
            }

            throw new FriendlyException(
                $"The build fails on drift: {DriftReportRenderer.Count(areas.Sum(x => x.Errors), "drift error")}. The drift error above lists them; resolve them until a run reports no drift.");
        }

        /// <summary>Runs each reporter in turn. One that throws is logged, naming it, and the others still run.</summary>
        [IntentManaged(Mode.Ignore)]
        internal static DriftCollector Check(IApplication application, IEnumerable<DriftReporterBase> reporters, ITracing log)
        {
            var collector = new DriftCollector();
            var context = new DriftContext(application, collector);
            foreach (var reporter in reporters)
            {
                try
                {
                    reporter.Run(context);
                }
                catch (Exception e)
                {
                    log.Failure(new FriendlyException($"The drift reporter {reporter.GetType().FullName} failed, so its checks are missing from this run: {e.GetType().Name}: {e.Message}"));
                }
            }

            return collector;
        }

        /// <summary>
        /// Logs the findings and, when there are any and the run writes the report, stages it. Nothing here may end the run:
        /// a failure is logged as one error instead.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        internal static void Publish(IApplication application, DriftCollector collector, ITracing log, bool writesReport)
        {
            try
            {
                var reportPath = writesReport ? DriftReports.RelativePath(application) : null;
                LogSummary(collector, log, application.OutputRootDirectory, reportPath);
                if (writesReport && collector.Findings.Any())
                {
                    application.ChangeManager.AddChange(
                        DriftReports.FullPath(application),
                        DriftReportRenderer.Render(application.GetApplicationConfig().Name, application.OutputRootDirectory, collector));
                }
            }
            catch (Exception e)
            {
                log.Failure(new FriendlyException($"The drift checks couldn't report their findings: {e.GetType().Name}: {e.Message}"));
            }
        }

        /// <summary>
        /// One error when the run has drift errors, else one warning when it has drift warnings: each Area's counts and the
        /// report's path. A run that writes no report (CI/CD) lists the findings in it instead, as its log is all there is.
        /// </summary>
        [IntentManaged(Mode.Ignore)]
        internal static void LogSummary(DriftCollector collector, ITracing log, string outputRoot, string reportPath)
        {
            var areas = collector.Areas.Where(x => x.Findings.Count > 0).ToList();
            if (areas.Count == 0)
            {
                return;
            }

            var errors = areas.Sum(x => x.Errors);
            var message = new StringBuilder($"Drift found: {DriftReportRenderer.Count(errors, "error")}, {DriftReportRenderer.Count(areas.Sum(x => x.Warnings), "warning")}.");
            if (reportPath != null)
            {
                message.Append($" Report: {reportPath}");
            }

            foreach (var area in areas)
            {
                message.Append($"\n- Drift in {area.Name}: {DriftReportRenderer.Count(area.Errors, "error")}, {DriftReportRenderer.Count(area.Warnings, "warning")}");
            }

            if (reportPath == null)
            {
                message.Append("\n\nFindings:");
                foreach (var area in areas)
                {
                    foreach (var finding in area.Findings)
                    {
                        var where = new[] { finding.Subject }.Concat(finding.Against).Select(x => Describe(x, outputRoot)).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        message.Append($"\n- {finding.Severity} [{area.Name}] {finding.Message}{(where.Count == 0 ? string.Empty : $" ({string.Join(", ", where)})")}");
                    }
                }
            }

            if (errors > 0)
            {
                log.Failure(new FriendlyException(message.ToString()));
            }
            else
            {
                // A warning is plain text: the Software Factory's output shows Markdown as literal characters.
                log.Warning(message.ToString().Replace("`", string.Empty));
            }
        }

        /// <summary>An anchor as the summary names it: an element by name, a file relative to the output root with its line.</summary>
        [IntentManaged(Mode.Ignore)]
        private static string Describe(DriftAnchor anchor, string outputRoot)
        {
            switch (anchor.Kind)
            {
                case DriftAnchorKind.Element:
                    return anchor.ModelElement.Name;
                case DriftAnchorKind.File:
                    var path = DriftReportRenderer.FullPath(anchor.FilePath, outputRoot);
                    var root = string.IsNullOrEmpty(outputRoot) ? null : DriftReportRenderer.Root(outputRoot);
                    if (root != null && path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    {
                        path = path[root.Length..];
                    }

                    return anchor.Line is { } line ? $"{path}:{line}" : path;
                default:
                    return anchor.Value;
            }
        }

        [IntentManaged(Mode.Ignore)]
        private static void DeleteLastReport(IApplication application)
        {
            var path = DriftReports.FullPath(application);
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Logging.Log.Warning($"The last drift report couldn't be deleted, so this run's may not be listed as a change: {e.Message}");
            }
        }

        [IntentManaged(Mode.Ignore)]
        private sealed class DriftRun
        {
            private readonly List<DriftReporterBase> _reporters = new();

            /// <summary>In their order, then by id, so the report is the same every run.</summary>
            public IReadOnlyList<DriftReporterBase> Reporters => _reporters.OrderBy(x => x.Order).ThenBy(x => x.Id, StringComparer.Ordinal).ToList();

            public void Register(DriftReporterBase reporter)
            {
                if (reporter != null && !_reporters.Contains(reporter))
                {
                    _reporters.Add(reporter);
                }
            }
        }
    }
}
