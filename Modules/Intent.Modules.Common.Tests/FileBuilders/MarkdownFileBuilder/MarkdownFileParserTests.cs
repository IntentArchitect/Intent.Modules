using System.Linq;
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

    [Fact]
    public void AWrappedBulletStaysOneItemWithItsContinuationIndented()
    {
        var result = RoundTrip("- **Most files** are generated output, produced from a model.\nEditing them by hand gets overwritten.\n- Next item");

        Assert.Contains("- **Most files** are generated output, produced from a model.\n  Editing them by hand gets overwritten.\n- Next item", Normalize(result));
    }

    [Fact]
    public void AWrappedOrderedItemIsIndentedToItsContentColumn()
    {
        var result = RoundTrip("1. **Version** — confirm the gate ran,\nincluding modules you did not anticipate.\n2. **Dependencies** — verify them.");

        Assert.Contains("1. **Version** — confirm the gate ran,\n   including modules you did not anticipate.\n2. **Dependencies**", Normalize(result));
    }

    [Fact]
    public void ABlankLineEndsTheItemAndStartsAParagraph()
    {
        var result = RoundTrip("- item\n\nA paragraph after the list.");

        Assert.Contains("- item\n\nA paragraph after the list.", Normalize(result));
    }

    [Theory]
    [InlineData("| a | b |")]
    [InlineData("> a quote")]
    [InlineData("===")]
    [InlineData("---")]
    [InlineData("1a. **A sub-step** written straight after its step.")]
    [InlineData("2) a parenthesised marker")]
    [InlineData("***")]
    [InlineData("___")]
    [InlineData("<details>")]
    [InlineData("<!-- a comment -->")]
    public void ATableRowQuoteRuleOrItemLikeMarkerNeverContinuesAnItem(string line)
    {
        var result = Normalize(RoundTrip("- item\n" + line));

        Assert.DoesNotContain("  " + line.Trim(), result);
        Assert.Contains("- item\n\n" + line.Trim(), result);
    }

    [Fact]
    public void AHeadingOrFenceAfterAnItemIsNotAContinuation()
    {
        var result = Normalize(RoundTrip("- item\n```\ncode\n```"));

        Assert.Contains("- item\n\n```\ncode\n```", result);
    }

    [Fact]
    public void AListIndentedAsAWholeIsATopLevelListNotACodeBlock()
    {
        var result = Normalize(RoundTrip("A paragraph:\n\n    - **Root modules** stand alone.\n    - **Bridging modules** connect two.\n      - nested"));

        Assert.Contains("\n- **Root modules** stand alone.\n- **Bridging modules** connect two.\n  - nested", result);
        Assert.DoesNotContain("    - **Root", result);
    }

    [Fact]
    public void SingleLineListsRoundTripUnchanged()
    {
        var result = Normalize(RoundTrip("- one\n- two\n  - nested\n\n1. first\n2. second"));

        Assert.Contains("- one\n- two\n  - nested\n", result);
        Assert.Contains("1. first\n2. second", result);
    }

    private static string Normalize(string text) => text.ReplaceLineEndings("\n");

    [Fact]
    public void AListAfterAParagraphInsideAnItemStaysNestedUnderThatItem()
    {
        var result = Normalize(RoundTrip("- Parent item\n\n  Explanation paragraph.\n\n  - child one\n  - child two\n- Next parent"));

        Assert.Contains("  - child one\n  - child two\n- Next parent", result);
    }

    [Fact]
    public void AWrappedLineKeepsATrailingHardBreak()
    {
        var result = Normalize(RoundTrip("- a\nsecond  \nthird"));

        Assert.Contains("- a\n  second  \n  third", result);
    }

    [Fact]
    public void AMultiLineItemBuiltThroughTheApiRendersAsBefore()
    {
        var file = new MarkdownFile("file", relativeLocation: "")
            .FromMarkdown("## Section\n\n- placeholder");
        file.Sections[0].Blocks.OfType<MarkdownListItem>().Single().Content = "Example:\n```csharp\nif (x)\n{\n    Foo();\n}\n```";

        Assert.Contains("- Example:\n```csharp\nif (x)\n{\n    Foo();\n}\n```", Normalize(file.ToString()));
    }
}
