using System;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.DriftReporting.FactoryExtensions;
using Intent.Modules.Common.DriftReporting.Reporting;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.DriftReporting.Tests
{
    /// <summary>Tests that set the Software Factory's static tracing, which the drift checks log through.</summary>
    [CollectionDefinition(nameof(LoggingCollection), DisableParallelization = true)]
    public class LoggingCollection
    {
    }

    [Collection(nameof(LoggingCollection))]
    public sealed class DriftChecksTests : IDisposable
    {
        private readonly ITracing _previous = Logging.Log;
        private readonly RecordingTracing _tracing = new();
        private readonly string _root = Path.Combine(Path.GetTempPath(), "drift-checks-" + Guid.NewGuid().ToString("N"));

        public DriftChecksTests()
        {
            Logging.SetTracing(_tracing);
        }

        public void Dispose()
        {
            Logging.SetTracing(_previous);

            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, recursive: true);
            }
        }

        private string SolutionRoot => Path.Combine(_root, "intent");
        private string ReportFile => Path.Combine(SolutionRoot, ".cache", "drift", "Acme", "DriftReport.mdx");

        private static void Step(object extension, IApplication application, string step) => ((IExecutionLifeCycle)extension).OnStep(application, step);

        /// <summary>A run's steps, as the Software Factory calls them: every extension's start, then every registration.</summary>
        private static void Run(IApplication application, DriftChecks checks, params DriftReporterBase[] reporters)
        {
            foreach (var extension in reporters.Cast<object>().Prepend(checks))
            {
                Step(extension, application, ExecutionLifeCycleSteps.Start);
            }

            foreach (var extension in reporters.Cast<object>().Prepend(checks))
            {
                Step(extension, application, ExecutionLifeCycleSteps.BeforeTemplateRegistrations);
            }

            foreach (var extension in reporters.Cast<object>().Prepend(checks))
            {
                Step(extension, application, ExecutionLifeCycleSteps.AfterTemplateExecution);
            }
        }

        [Fact]
        public void Every_registered_reporter_runs_in_order_and_the_report_is_staged()
        {
            var application = Fakes.Application(SolutionRoot, _root);
            var calls = new System.Collections.Generic.List<string>();
            var architecture = new FakeReporter("Arch", 20, c =>
            {
                calls.Add("Arch");
                c.Area("Architecture", DriftCategory.CodeCode).Log(new DriftFinding(DriftCategory.CodeCode, DriftSeverity.Error, "imports", DriftAnchor.File("src/A.java", 3)));
            });
            var domain = new FakeReporter("Domain", 10, c =>
            {
                calls.Add("Domain");
                c.Area("Domain", DriftCategory.ModelCode);
            });

            Run(application, new DriftChecks(), architecture, domain);

            calls.ShouldBe(new[] { "Domain", "Arch" });
            _tracing.Lines.ShouldBe(new[] { "FriendlyException: Drift found: 1 error, 0 warnings. Report: intent/.cache/drift/Acme/DriftReport.mdx\n- Drift in Architecture: 1 error, 0 warnings" });
            application.ChangeManager.Received(1).AddChange(Path.GetFullPath(ReportFile), Arg.Is<string>(x => x.StartsWith("# Drift report: Acme\n")));
        }

        [Fact]
        public void A_reporter_that_throws_is_logged_and_the_others_still_run()
        {
            var application = Fakes.Application(SolutionRoot, _root);
            var broken = new FakeReporter("Broken", 10, _ => throw new InvalidOperationException("no recipe"));
            var working = new FakeReporter("Working", 20, c =>
                c.Area("Domain", DriftCategory.ModelCode).Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Warning, "stale", DriftAnchor.Text("x"))));

            Run(application, new DriftChecks(), broken, working);

            working.Calls.ShouldBe(1);
            _tracing.Lines[0].ShouldBe($"FriendlyException: The drift reporter {typeof(FakeReporter).FullName} failed, so its checks are missing from this run: InvalidOperationException: no recipe");
            _tracing.Lines.ShouldContain("Warning: Drift found: 0 errors, 1 warning. Report: intent/.cache/drift/Acme/DriftReport.mdx\n- Drift in Domain: 0 errors, 1 warning");
        }

        [Fact]
        public void A_clean_run_stages_nothing_and_deletes_the_last_report()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ReportFile)!);
            File.WriteAllText(ReportFile, "# Drift report: Acme\n");
            var application = Fakes.Application(SolutionRoot, _root);
            var clean = new FakeReporter("Clean", 10, c => c.Area("Domain", DriftCategory.ModelCode));

            Run(application, new DriftChecks(), clean);

            clean.Calls.ShouldBe(1);
            File.Exists(ReportFile).ShouldBeFalse();
            application.ChangeManager.DidNotReceiveWithAnyArgs().AddChange(default, default);
            _tracing.Lines.ShouldBeEmpty();
        }

        [Fact]
        public void In_a_CI_run_the_findings_are_logged_no_report_is_written_and_a_drift_error_breaks_the_run()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ReportFile)!);
            File.WriteAllText(ReportFile, "# Drift report: Acme\n");
            var application = Fakes.Application(SolutionRoot, _root);
            var checks = new DriftChecks();
            var reporter = new FakeReporter("Domain", 10, c =>
                c.Area("Domain", DriftCategory.ModelCode).Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "drift", DriftAnchor.Text("x"))));

            checks.Start(application, onCi: true);
            Step(reporter, application, ExecutionLifeCycleSteps.BeforeTemplateRegistrations);
            Should.Throw<FriendlyException>(() => checks.Finish(application, onCi: true));

            reporter.Calls.ShouldBe(1);
            _tracing.Lines.ShouldBe(new[] { "FriendlyException: Drift found: 1 error, 0 warnings.\n- Drift in Domain: 1 error, 0 warnings\n\nFindings:\n- Error [Domain] drift (x)" });
            File.Exists(ReportFile).ShouldBeTrue("a CI/CD run neither deletes the last report nor writes one");
            application.ChangeManager.DidNotReceiveWithAnyArgs().AddChange(default, default);
        }

        [Fact]
        public void In_a_CI_run_a_drift_error_breaks_the_run()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.ModelCode }).Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "drift", DriftAnchor.Text("x")));
            collector.Area("Domain", new[] { DriftCategory.ModelCode }).Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Warning, "stale", DriftAnchor.Text("y")));
            collector.Area("Architecture", new[] { DriftCategory.CodeCode }).Log(new DriftFinding(DriftCategory.CodeCode, DriftSeverity.Error, "imports", DriftAnchor.Text("z")));

            Should.Throw<FriendlyException>(() => DriftChecks.FailOnCi(collector, onCi: true))
                .Message.ShouldBe("The build fails on drift: 2 drift errors. The drift error above lists them; resolve them until a run reports no drift.");
        }

        [Fact]
        public void In_a_CI_run_warnings_alone_dont_break_the_run()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.Code }).Log(new DriftFinding(DriftCategory.Code, DriftSeverity.Warning, "stale", DriftAnchor.Text("x")));

            Should.NotThrow(() => DriftChecks.FailOnCi(collector, onCi: true));
        }

        [Fact]
        public void Outside_a_CI_run_a_drift_error_doesnt_end_the_run()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.ModelCode }).Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "drift", DriftAnchor.Text("x")));

            Should.NotThrow(() => DriftChecks.FailOnCi(collector, onCi: false));
        }

        [Fact]
        public void With_no_reporters_nothing_runs()
        {
            var application = Fakes.Application(SolutionRoot, _root);

            Run(application, new DriftChecks());

            application.ChangeManager.DidNotReceiveWithAnyArgs().AddChange(default, default);
            _tracing.Lines.ShouldBeEmpty();
        }

        [Fact]
        public void The_report_path_is_relative_to_the_folder_holding_the_solution()
        {
            DriftReports.RelativePath(Fakes.Application(SolutionRoot, _root)).ShouldBe("intent/.cache/drift/Acme/DriftReport.mdx");
        }

        [Fact]
        public void A_CI_run_is_read_from_the_CLIs_parameter_and_a_host_without_parameters_never_is_one()
        {
            DriftChecks.CiParameter.ShouldBe("ensure-no-outstanding-changes");
            DriftChecks.IsCi(Substitute.For<ISoftwareFactoryExecutionContext>()).ShouldBeFalse();
        }
    }
}
