using System.Linq;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.DriftReporting.Reporting;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.DriftReporting.Tests
{
    public class CollectorTests
    {
        private static DriftFinding Finding(DriftCategory category, DriftSeverity severity = DriftSeverity.Error) =>
            new(category, severity, "drift", DriftAnchor.Text("subject"));

        [Fact]
        public void Areas_keep_the_order_they_were_first_asked_for_in()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.ModelCode });
            collector.Area("Architecture", new[] { DriftCategory.CodeCode });
            collector.Area("Domain", new[] { DriftCategory.Code });

            collector.Areas.Select(x => x.Name).ShouldBe(new[] { "Domain", "Architecture" });
        }

        [Fact]
        public void Reporters_that_share_an_Area_add_their_declarations_and_findings_together()
        {
            var collector = new DriftCollector();
            var first = collector.Area("Domain", new[] { DriftCategory.ModelCode });
            var second = collector.Area("Domain", new[] { DriftCategory.Code });
            first.Log(Finding(DriftCategory.ModelCode));
            second.Log(Finding(DriftCategory.ModelCode, DriftSeverity.Warning));

            second.ShouldBeSameAs(first);
            first.Categories.ShouldBe(new[] { DriftCategory.ModelCode, DriftCategory.Code });
            first.Errors.ShouldBe(1);
            first.Warnings.ShouldBe(1);
        }

        [Fact]
        public void A_category_shows_when_declared_or_when_it_has_findings_in_a_fixed_order()
        {
            var collector = new DriftCollector();
            var area = collector.Area("Architecture", new[] { DriftCategory.Code });
            area.Log(Finding(DriftCategory.CodeCode));

            area.Categories.ShouldBe(new[] { DriftCategory.CodeCode, DriftCategory.Code });
        }

        [Fact]
        public void A_declared_category_with_no_findings_is_clean_and_one_not_declared_is_left_out()
        {
            var collector = new DriftCollector();
            collector.Area("Domain", new[] { DriftCategory.ModelCode, DriftCategory.CodeCode }).Log(Finding(DriftCategory.ModelCode));

            var report = DriftReportRenderer.Render("Acme", "D:/Repos/Acme", collector);

            report.ShouldContain("### Model / Code drift (1)");
            report.ShouldContain("### Code / Code drift\n\n✓ Clean");
            report.ShouldNotContain("### Code drift");
        }
    }
}
