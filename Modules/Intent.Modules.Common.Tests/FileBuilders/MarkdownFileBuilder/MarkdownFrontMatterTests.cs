using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Xunit;

namespace Intent.Modules.Common.Tests.FileBuilders.MarkdownFileBuilder;

public class MarkdownFrontMatterTests
{
    private static string RoundTrip(string markdown) =>
        new MarkdownFile("file", relativeLocation: "").FromMarkdown(markdown).ToString().ReplaceLineEndings("\n");

    private static MarkdownFile Parse(string markdown) =>
        (MarkdownFile)new MarkdownFile("file", relativeLocation: "").FromMarkdown(markdown);

    [Fact]
    public void AnIndentedBlockListSurvivesARoundTrip()
    {
        const string markdown = "---\nname: page\ndescription: Use when adding a page.\npaths:\n  - \"**/*.razor\"\n  - \"**/*.razor.cs\"\n---\n# Page\n\nBody.\n";

        Assert.Equal(markdown, RoundTrip(markdown));
    }

    [Fact]
    public void ABlockListAtTheKeysOwnIndentSurvivesARoundTrip()
    {
        const string markdown = "---\npaths:\n- \"**/*.razor\"\n- \"**/*.razor.cs\"\nname: page\n---\n# Page\n";

        Assert.Equal(markdown, RoundTrip(markdown));
    }

    [Fact]
    public void AnInlineListSurvivesARoundTrip()
    {
        const string markdown = "---\nglobs: [\"**/*.cs\", '**/*.csproj']\n---\n# Page\n";

        Assert.Equal(markdown, RoundTrip(markdown));
    }

    [Fact]
    public void ANestedMapStaysUnderItsKeyInsteadOfBeingLiftedOut()
    {
        const string markdown = "---\nname: skill\nmetadata:\n  author: someone\n  version: \"1.0\"\n---\n# Skill\n";

        var file = Parse(markdown);

        Assert.Equal(markdown, file.ToString().ReplaceLineEndings("\n"));
        Assert.False(file.FrontMatter.HasKey("author"));
        Assert.Null(file.FrontMatter.GetList("metadata"));
    }

    [Fact]
    public void AMultiLineDescriptionSurvivesARoundTrip()
    {
        const string markdown = "---\ndescription: >-\n  First line.\n\n  Second paragraph, USE FOR: things.\nname: skill\n---\n# Skill\n";

        var file = Parse(markdown);

        Assert.Equal(markdown, file.ToString().ReplaceLineEndings("\n"));
        Assert.False(file.FrontMatter.HasKey("Second paragraph, USE FOR"));
    }

    [Fact]
    public void ScalarFrontMatterRendersExactlyAsBefore()
    {
        // One-line values, including an empty one, keep their "key: value" form byte for byte.
        const string markdown = "---\nname: skill\ndescription: Does a thing: well.\nempty: \nflag: true\n---\n# Skill\n";

        Assert.Equal(markdown, RoundTrip(markdown));
    }

    [Fact]
    public void BlankLinesBetweenScalarKeysAreDroppedAsBefore()
    {
        Assert.Equal("---\na: 1\nb: 2\n---\n# S\n", RoundTrip("---\na: 1\n\nb: 2\n\n---\n# S\n"));
    }

    [Fact]
    public void GetListReadsBlockAndInlineListsUnquoted()
    {
        var file = Parse("---\npaths:\n  - \"**/*.razor\"\n  - plain # a comment\n  - 'it''s'\nglobs: [a, \"b, c\"]\nname: x\n---\n# S\n");

        Assert.Equal(new[] { "**/*.razor", "plain", "it's" }, file.FrontMatter.GetList("paths"));
        Assert.Equal(new[] { "a", "b, c" }, file.FrontMatter.GetList("globs"));
        Assert.Null(file.FrontMatter.GetList("name"));
        Assert.Null(file.FrontMatter.GetList("missing"));
        Assert.Equal(string.Empty, file.FrontMatter.Get("paths"));
    }

    [Fact]
    public void SetListWritesABlockListQuotingOnlyWhatYamlNeeds()
    {
        var file = new MarkdownFile("file", relativeLocation: "");
        file.FrontMatter
            .Set("name", "skill")
            .SetList("paths", new[] { "**/*.razor", "src/Pages", "true", "a: b", "say \"hi\"" });

        Assert.Equal(
            "---\nname: skill\npaths:\n  - \"**/*.razor\"\n  - src/Pages\n  - \"true\"\n  - \"a: b\"\n  - say \"hi\"\n---\n",
            file.ToString().ReplaceLineEndings("\n"));
        Assert.Equal(new[] { "**/*.razor", "src/Pages", "true", "a: b", "say \"hi\"" }, file.FrontMatter.GetList("paths"));
    }

    [Fact]
    public void AQuotedItemWithEscapesRoundTripsThroughSetListAndGetList()
    {
        var file = new MarkdownFile("file", relativeLocation: "");
        file.FrontMatter.SetList("items", new[] { "\"quoted\" start", "back\\slash: here" });

        Assert.Contains("  - \"\\\"quoted\\\" start\"\n  - \"back\\\\slash: here\"\n", file.ToString().ReplaceLineEndings("\n"));
        Assert.Equal(new[] { "\"quoted\" start", "back\\slash: here" }, file.FrontMatter.GetList("items"));
    }

    [Fact]
    public void SetListWithNoValuesWritesAnEmptyInlineList()
    {
        var file = new MarkdownFile("file", relativeLocation: "");
        file.FrontMatter.SetList("paths", new string[0]);

        Assert.Equal("---\npaths: []\n---\n", file.ToString().ReplaceLineEndings("\n"));
        Assert.Empty(file.FrontMatter.GetList("paths"));
    }

    [Fact]
    public void SetReplacesAParsedListWithAScalarInPlace()
    {
        var file = Parse("---\npaths:\n  - a\nname: x\n---\n# S\n");
        file.FrontMatter.Set("paths", "b");

        Assert.Equal("---\npaths: b\nname: x\n---\n# S\n", file.ToString().ReplaceLineEndings("\n"));
    }

    [Fact]
    public void AnExtensionCanAddAnItemToAListWrittenInTheRawString()
    {
        var file = Parse("---\nname: x\npaths:\n- \"**/*.razor\"\n- \"**/*.razor.cs\"\ndescription: d\n---\n# S\n");
        var frontMatter = file.FrontMatter;

        frontMatter.SetList("paths", [.. frontMatter.GetList("paths") ?? [], "**/*.cshtml"]);

        Assert.Equal(
            "---\nname: x\npaths:\n  - \"**/*.razor\"\n  - \"**/*.razor.cs\"\n  - \"**/*.cshtml\"\ndescription: d\n---\n# S\n",
            file.ToString().ReplaceLineEndings("\n"));
    }

    [Fact]
    public void SetListOnParsedFrontMatterAppendsAfterTheExistingKeys()
    {
        var file = Parse("---\nname: x\n---\n# S\n");
        file.FrontMatter.SetList("paths", new[] { "**/*.cs" });

        Assert.Equal("---\nname: x\npaths:\n  - \"**/*.cs\"\n---\n# S\n", file.ToString().ReplaceLineEndings("\n"));
    }
}
