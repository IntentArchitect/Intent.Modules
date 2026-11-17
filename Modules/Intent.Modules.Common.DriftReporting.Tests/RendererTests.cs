using System;
using System.IO;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.DriftReporting.Reporting;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.DriftReporting.Tests
{
    public class RendererTests
    {
        /// <summary>Every shape the report has, with text that would break MDX if it weren't escaped.</summary>
        internal static DriftCollector Sample()
        {
            var collector = new DriftCollector();
            var domain = collector.Area("Domain Design", new[] { DriftCategory.ModelCode, DriftCategory.CodeCode });
            domain.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Error,
                "Modelled as `decimal`, but the field `unitPrice` is a `String`.",
                DriftAnchor.Element(Fakes.Element("attr-unit-price", "UnitPrice")),
                DriftAnchor.File("src/main/java/com/eshop/backend/domain/Product.java", 24)));
            domain.Log(new DriftFinding(DriftCategory.ModelCode, DriftSeverity.Warning,
                "The repository extends JpaRepository<Order, Long> and maps {id}; `JpaRepository<Order, Long>` and `a|b` are code, a|b isn't.",
                DriftAnchor.Element(Fakes.Element("class-order", "Order")),
                DriftAnchor.File("D:/Repos/Acme/src/main/java/com/eshop/backend/domain/OrderRepository.java", 7),
                DriftAnchor.Text("`src/main/java/com/eshop/backend/domain`")));

            var architecture = collector.Area("Architecture", new[] { DriftCategory.ModelCode, DriftCategory.CodeCode, DriftCategory.Code });
            architecture.Log(new DriftFinding(DriftCategory.CodeCode, DriftSeverity.Error,
                "Imports `infrastructure.DevDataSeeder`, and Application has no May Depend On arrow to Infrastructure.",
                DriftAnchor.File("src/main/java/com/eshop/backend/application/ProductService.java", 9),
                DriftAnchor.Element(Fakes.Element("layer-application", "Application")),
                DriftAnchor.Element(Fakes.Element("layer-infrastructure", "Infrastructure"))) { Rule = "arch.layer-dependency" });
            architecture.Log(new DriftFinding(DriftCategory.Code, DriftSeverity.Warning,
                "A type in `api` is not a `*Controller`, so no convention covers it: an unmatched ` backtick stays literal.",
                DriftAnchor.File("src/main/java/com/eshop/backend/api/CaseInsensitiveQueryFilter.java")));

            collector.Area("Services", new[] { DriftCategory.ModelCode });
            return collector;
        }

        [Fact]
        public void The_report_matches_its_golden_file()
        {
            var report = DriftReportRenderer.Render("eShop.Backend", "D:/Repos/Acme", Sample());

            Golden.ShouldMatch("DriftReport.mdx", report);
        }

        [Theory]
        [InlineData("JpaRepository<Order, Long>", "JpaRepository&lt;Order, Long&gt;")]
        [InlineData("maps {id}", "maps &#123;id&#125;")]
        [InlineData("a|b", "a\\|b")]
        [InlineData("`a|b`", "`a\\|b`")]
        [InlineData("`JpaRepository<Order, Long>` and `{id}`", "`JpaRepository<Order, Long>` and `{id}`")]
        [InlineData("one ` backtick", "one \\` backtick")]
        [InlineData("see ```List<Order>` here", "see \\`\\`\\`List&lt;Order&gt;\\` here")]
        [InlineData("value `<T>``` end", "value \\`&lt;T&gt;\\`\\`\\` end")]
        [InlineData("a ``b`c`` d", "a ``b`c`` d")]
        [InlineData("pattern `a\\|<b>` here", "pattern \\`a\\\\\\|&lt;b&gt;\\` here")]
        [InlineData("an empty `` span", "an empty \\`\\` span")]
        [InlineData("*not* _emphasis_ [or a link]", "\\*not\\* \\_emphasis\\_ \\[or a link\\]")]
        [InlineData("two\nlines", "two lines")]
        public void Plain_text_is_escaped_for_MDX_except_inside_backticks(string plain, string mdx)
        {
            DriftReportRenderer.Text(plain).ShouldBe(mdx);
        }

        [Fact]
        public void A_heading_links_by_its_slug()
        {
            DriftReportRenderer.Slug("Service Designer Drift").ShouldBe("service-designer-drift");
            DriftReportRenderer.Slug("UI").ShouldBe("ui");
        }
    }

    internal static class Golden
    {
        /// <summary>Compares with <c>Fixtures/Golden</c>; run with <c>DRIFT_GOLDEN=update</c> to capture the actual text instead.</summary>
        public static void ShouldMatch(string relativePath, string actual)
        {
            if (Environment.GetEnvironmentVariable("DRIFT_GOLDEN") == "update")
            {
                var path = Path.Combine(TestsFolder(), "Fixtures", "Golden", relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.WriteAllText(path, actual);
                return;
            }

            var golden = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Golden", relativePath);
            File.Exists(golden).ShouldBeTrue($"No golden file {relativePath}: run the tests with DRIFT_GOLDEN=update to capture it.");
            actual.ShouldBe(File.ReadAllText(golden).Replace("\r\n", "\n"));
        }

        private static string TestsFolder()
        {
            var folder = new DirectoryInfo(AppContext.BaseDirectory);
            while (folder != null && !File.Exists(Path.Combine(folder.FullName, "Intent.Modules.Common.DriftReporting.Tests.csproj")))
            {
                folder = folder.Parent;
            }

            return folder?.FullName ?? throw new InvalidOperationException("The tests' project folder wasn't found.");
        }
    }
}
