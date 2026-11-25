using System.Linq;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Xunit;

namespace Intent.Modules.Common.Tests.FileBuilders.MarkdownFileBuilder;

public class MarkdownSectionWithListItemsTests
{
    private static string Render(string markdownList)
    {
        var file = new MarkdownFile("file", relativeLocation: "").FromMarkdown("## Section\n\n- existing\n");
        file.FindSection("Section").WithListItems(markdownList);
        return file.ToString().ReplaceLineEndings("\n");
    }

    [Fact]
    public void SingleLineItemsRenderAsBefore()
    {
        var result = Render("- one\n- two\n  - nested\n1. first\n2. second");

        Assert.Equal("## Section\n\n- existing\n- one\n- two\n  - nested\n1. first\n2. second\n", result);
    }

    [Fact]
    public void AWrappedItemKeepsItsContinuationLines()
    {
        var result = Render("- **Gate:** If you write `new XxxDto`,\n  or call any mapper, you are blocked\n  until one branch is complete:\n    - **Branch A:** locate a mapper\n      with the required method.\n- Next");

        Assert.Contains(
            "- **Gate:** If you write `new XxxDto`,\n  or call any mapper, you are blocked\n  until one branch is complete:\n" +
            "    - **Branch A:** locate a mapper\n      with the required method.\n- Next",
            result);
    }

    [Fact]
    public void AnEmphasisLedLineIsNotTreatedAsABullet()
    {
        var result = Render("- item\n\n**Note** this line starts with bold.");

        Assert.Contains("\n**Note** this line starts with bold.", result);
        Assert.DoesNotContain("- *Note**", result);
    }

    [Fact]
    public void ARuleIsNotTreatedAsABullet()
    {
        var result = Render("- item\n---");

        Assert.Contains("- item\n\n---", result);
        Assert.DoesNotContain("- --", result);
    }

    [Fact]
    public void AHeadingLineIsKeptAsTextAndDoesNotContinueAnItem()
    {
        var file = new MarkdownFile("file", relativeLocation: "").FromMarkdown("## Section\n");
        file.FindSection("Section").WithListItems("- item\n### Not a section");

        Assert.Single(file.Sections);
        Assert.Contains("- item\n\n### Not a section", file.ToString().ReplaceLineEndings("\n"));
    }

    [Fact]
    public void AWhitespaceOnlyLastLineEndsTheItemAndIsDropped()
    {
        // The shape of Application.Dtos.Mapperly's AISkillsExtension raw string: its last content line
        // holds only the six spaces left after the raw string's closing-quote indent is removed.
        var file = new MarkdownFile("file", relativeLocation: "").FromMarkdown("## Section\n");
        var section = file.FindSection("Section");
        section.WithListItems(
            "- If you can't find any existing mappings, create them in the same project as the services under:\n" +
            "    - `./Mappings/<FeatureOrAggregate>/<Entity>DtoMapper.cs`\n" +
            "      ");
        section.WithCodeBlock("var x = 1;", "csharp");

        Assert.Equal(
            "## Section\n\n" +
            "- If you can't find any existing mappings, create them in the same project as the services under:\n" +
            "    - `./Mappings/<FeatureOrAggregate>/<Entity>DtoMapper.cs`\n\n" +
            "```csharp\nvar x = 1;\n```\n",
            file.ToString().ReplaceLineEndings("\n"));
        Assert.Equal("`./Mappings/<FeatureOrAggregate>/<Entity>DtoMapper.cs`", section.ListItems[^1].Content);
    }

    [Fact]
    public void WhitespaceOnlyLinesAroundAParagraphAreTrimmed()
    {
        var result = Render("- item\n   \nA paragraph.\n  \n");

        Assert.EndsWith("- item\n\nA paragraph.\n", result);
    }

    [Fact]
    public void ItemsAreAppendedAfterTheSectionsExistingBlocks()
    {
        var file = new MarkdownFile("file", relativeLocation: "").FromMarkdown("## Section\n\nIntro.\n");
        var section = file.FindSection("Section");
        section.WithListItems("- a\n- b");

        Assert.Equal(new[] { "a", "b" }, section.ListItems.Select(i => i.Content));
        Assert.IsType<MarkdownTextBlock>(section.Blocks[0]);
    }
}
