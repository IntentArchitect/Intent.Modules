using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.DriftReporting.FactoryExtensions;
using Intent.Modules.Common.DriftReporting.Reporting;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.DriftReporting.Tests
{
    public class SummaryTests
    {
        private const string Report = "intent/.cache/drift/Acme/DriftReport.mdx";

        /// <summary>The brownfield harness reads each Area's errors from this, in <c>Tests.Brownfield/run.mjs</c>.</summary>
        private static readonly Regex HarnessDriftError = new(@"Drift in ([^:\n]+): (\d+) errors?");

        [Fact]
        public void A_run_with_drift_errors_logs_one_error_with_each_Areas_counts_and_the_report()
        {
            var collector = new DriftCollector();
            var domain = collector.Area("Domain", new[] { DriftCategory.ModelCode });
            domain.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "Modelled as `decimal`, code `string`",
                DriftAnchor.Element(Fakes.Element("attr-1", "Total")), DriftAnchor.File("src/Order.cs", 12)));
            domain.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Warning, "stale", DriftAnchor.Text("x")));
            collector.Area("Architecture", new[] { DriftCategory.CodeCode }).Log(new DriftFinding(DriftCategory.CodeCode, DriftSeverity.Error, "imports", DriftAnchor.File("src/A.java", 3)));
            collector.Area("Services", new[] { DriftCategory.ModelCode });
            var tracing = new RecordingTracing();

            DriftChecks.LogSummary(collector, tracing, "D:/Repos/Acme", Report);

            tracing.Lines.ShouldBe(new[]
            {
                $"FriendlyException: Drift found: 2 errors, 1 warning. Report: {Report}\n- Drift in Domain: 1 error, 1 warning\n- Drift in Architecture: 1 error, 0 warnings"
            });
        }

        [Fact]
        public void A_run_with_only_drift_warnings_logs_one_warning()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.Code }).Log(new DriftFinding(DriftCategory.Code, DriftSeverity.Warning, "`Order.Legacy` matches nothing", DriftAnchor.Text("x")));
            var tracing = new RecordingTracing();

            DriftChecks.LogSummary(collector, tracing, "D:/Repos/Acme", Report);

            tracing.Lines.ShouldBe(new[] { $"Warning: Drift found: 0 errors, 1 warning. Report: {Report}\n- Drift in Domain: 0 errors, 1 warning" });
            tracing.Failures.ShouldBeEmpty();
        }

        [Fact]
        public void A_run_with_no_findings_logs_nothing()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.ModelCode });
            var tracing = new RecordingTracing();

            DriftChecks.LogSummary(collector, tracing, "D:/Repos/Acme", Report);

            tracing.Lines.ShouldBeEmpty();
        }

        [Fact]
        public void Without_a_report_the_one_error_lists_every_finding_with_files_relative_to_the_output_root()
        {
            var root = Path.Combine(Path.GetTempPath(), "Acme");
            var collector = new DriftCollector();
            var area = collector.Area("Domain", new[] { DriftCategory.ModelCode });
            area.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "Modelled as `decimal`",
                DriftAnchor.Element(Fakes.Element("attr-1", "Total")), DriftAnchor.File(Path.Combine(root, "src", "Order.java"), 12)));
            area.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Warning, "stale", DriftAnchor.Text("x")));
            var tracing = new RecordingTracing();

            DriftChecks.LogSummary(collector, tracing, Path.Combine(root, "intent", "..", "..", "Acme"), reportPath: null);

            tracing.Lines.ShouldBe(new[]
            {
                "FriendlyException: Drift found: 1 error, 1 warning.\n- Drift in Domain: 1 error, 1 warning\n\nFindings:\n- Error [Domain] Modelled as `decimal` (Total, src/Order.java:12)\n- Warning [Domain] stale (x)"
            });
        }

        [Fact]
        public void The_brownfield_harness_reads_each_Areas_errors_from_the_one_error()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.ModelCode }).Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "a", DriftAnchor.Text("x")));
            collector.Area("Services", new[] { DriftCategory.ModelCode });
            var design = collector.Area("Domain Design", new[] { DriftCategory.ModelCode });
            design.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "b", DriftAnchor.Text("y")));
            design.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error, "c", DriftAnchor.Text("z")));
            var tracing = new RecordingTracing();

            DriftChecks.LogSummary(collector, tracing, "D:/Repos/Acme", Report);

            tracing.Failures.Count.ShouldBe(1);
            HarnessDriftError.Matches(tracing.Lines.Single()).Select(m => $"{m.Groups[1].Value}={m.Groups[2].Value}")
                .ShouldBe(new[] { "Domain=1", "Domain Design=2" });
        }
    }
}
