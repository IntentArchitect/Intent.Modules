using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Xunit;

namespace Intent.Modules.Common.Tests.FileBuilders.MarkdownFileBuilder;

public class MarkdownFileParserTests
{
    // Content only renders when it sits under a heading, so every case gets one.
    private static string RoundTrip(string markdown) =>
        new MarkdownFile("file", relativeLocation: "")
            .FromMarkdown("## Section\n\n" + markdown)
            .ToString();

    [Theory]
    [InlineData("**Check whether the module wants them.** On the module's package, read it first.")]
    [InlineData("*pointer* is the term used for the mapping accessors.")]
    [InlineData("***strongly emphasised*** opening a paragraph.")]
    public void EmphasisOpeningAParagraphIsNotTreatedAsABullet(string line)
    {
        var result = RoundTrip(line);

        Assert.Contains(line, result);
        Assert.DoesNotContain("- *", result);
    }

    [Theory]
    [InlineData("- dash bullet")]
    [InlineData("* asterisk bullet")]
    [InlineData("+ plus bullet")]
    public void RealBulletsAreStillParsed(string line)
    {
        var result = RoundTrip(line);

        Assert.Contains("- " + line[2..], result);
    }

    [Fact]
    public void ABulletWhoseTextStartsWithEmphasisKeepsBothTheBulletAndTheEmphasis()
    {
        var result = RoundTrip("- **Ticked, file missing** — create it.");

        Assert.Contains("- **Ticked, file missing** — create it.", result);
    }

    [Fact]
    public void NestedBulletsRetainTheirIndentation()
    {
        var result = RoundTrip("- outer\n  - inner");

        Assert.Contains("- outer", result);
        Assert.Contains("  - inner", result);
    }
}
