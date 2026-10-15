using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Modelers.CodebaseStructure.Migrations;
using Shouldly;
using Xunit;
using Ids = Intent.Modules.Modelers.CodebaseStructure.Migrations.Migration_01_02_00_Pre_00.Ids;

namespace Intent.Modules.Modelers.CodebaseStructure.Tests.Migrations;

public class Migration_01_02_00_Pre_00Tests
{
    private const string DataAccessId = "11111111-1111-1111-1111-111111111111";
    private const string PciScopeId = "22222222-2222-2222-2222-222222222222";

    private static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
    {
        [DataAccessId] = "Data Access",
        [PciScopeId] = "PCI scope"
    };

    /// <summary>
    /// The migration writes these into consumers' metadata, so a typo silently orphans data. They must equal
    /// this module's own Module Builder ids, mirrored by the client's ClassificationStereotypeConstants.
    /// </summary>
    [Fact]
    public void Ids_match_the_module_metadata()
    {
        Ids.DefinitionPackageId.ShouldBe("525868ff-adc0-4ab6-a90e-352288da941f");
        Ids.TemplateOutputSpecializationTypeId.ShouldBe("d421c322-7a51-4094-89fa-e5d8a0a97b27");
        Ids.FileClassificationSpecializationTypeId.ShouldBe("86490efd-d823-44cf-bb67-283231ae95f3");

        Ids.OutputClassification.DefinitionId.ShouldBe("98cff892-b03e-4d9c-b1dc-031cb0c900ee");
        Ids.OutputClassification.ClassificationId.ShouldBe("0acdff86-3981-4950-921c-f5642a3e2374");
        Ids.OutputClassification.ReviewImportanceId.ShouldBe("277dec79-e6f2-4e4a-ab8e-1849990889a1");

        Ids.FileClassification.DefinitionId.ShouldBe("1a5d919b-7ca2-4f2f-8a0d-0995aa6b02ac");
        Ids.FileClassification.EntriesId.ShouldBe("3e552682-b2c7-450a-acb6-be006206cf97");
        Ids.FileClassification.EntryDefinitionId.ShouldBe("14476d2c-c5fb-4aea-ad8c-55d1b00a8203");
        Ids.FileClassification.GlobId.ShouldBe("14dd36e5-eee3-4b8f-9868-99b965703d23");
        Ids.FileClassification.ReviewImportanceId.ShouldBe("1f903649-4ed0-4c2b-b288-34dedacdff05");
        Ids.FileClassification.ClassificationId.ShouldBe("c42e8c3a-962a-4e85-b890-e3234eb6534d");
    }

    [Fact]
    public void Up_converts_output_classification_into_a_single_match_all_entry()
    {
        var element = NewTemplateOutput(WithOutputClassification("2 - Medium", $"[\"{DataAccessId}\",\"{PciScopeId}\"]"));

        Migration_01_02_00_Pre_00.TryConvertToEntries(element, Names).ShouldBeTrue();

        element.Stereotypes.ShouldNotContain(x => x.DefinitionId == Ids.OutputClassification.DefinitionId);
        var custom = element.Stereotypes.Single(x => x.DefinitionId == Ids.FileClassification.DefinitionId);
        custom.DefinitionPackageId.ShouldBe(Ids.DefinitionPackageId);

        var entries = custom.Properties.Single(x => x.DefinitionId == Ids.FileClassification.EntriesId);
        entries.Value.ShouldBe("[\"[Medium - Data Access, PCI scope] **/*\"]");

        var item = entries.ItemList.ShouldHaveSingleItem();
        item.DefinitionId.ShouldBe(Ids.FileClassification.EntryDefinitionId);
        item.DefinitionPackageId.ShouldBe(Ids.DefinitionPackageId);
        Value(item, Ids.FileClassification.GlobId).ShouldBe("**/*");
        Value(item, Ids.FileClassification.ReviewImportanceId).ShouldBe("2 - Medium");
        Value(item, Ids.FileClassification.ClassificationId).ShouldBe($"[\"{DataAccessId}\",\"{PciScopeId}\"]");
    }

    [Fact]
    public void Up_leaves_an_element_that_already_has_file_classification()
    {
        var element = NewTemplateOutput(
            WithOutputClassification("2 - Medium", $"[\"{DataAccessId}\"]"),
            new StereotypePersistable { DefinitionId = Ids.FileClassification.DefinitionId, Name = Ids.FileClassification.Name, Properties = [] });

        Migration_01_02_00_Pre_00.TryConvertToEntries(element, Names).ShouldBeFalse();

        element.Stereotypes.Count.ShouldBe(2);
    }

    [Fact]
    public void Up_drops_an_empty_output_classification_without_adding_entries()
    {
        var element = NewTemplateOutput(WithOutputClassification(null, "[]"));

        Migration_01_02_00_Pre_00.TryConvertToEntries(element, Names).ShouldBeTrue();

        element.Stereotypes.ShouldBeEmpty();
    }

    [Fact]
    public void Down_reverses_up()
    {
        var element = NewTemplateOutput(WithOutputClassification("3 - High", $"[\"{PciScopeId}\"]"));
        Migration_01_02_00_Pre_00.TryConvertToEntries(element, Names);

        Migration_01_02_00_Pre_00.TryRevertToOutputClassification(element, Names).ShouldBeTrue();

        var legacy = element.Stereotypes.ShouldHaveSingleItem();
        legacy.DefinitionId.ShouldBe(Ids.OutputClassification.DefinitionId);
        Value(legacy, Ids.OutputClassification.ReviewImportanceId).ShouldBe("3 - High");
        Value(legacy, Ids.OutputClassification.ClassificationId).ShouldBe($"[\"{PciScopeId}\"]");
    }

    [Fact]
    public void Down_keeps_entries_a_user_has_added_to()
    {
        var element = NewTemplateOutput(WithOutputClassification("2 - Medium", $"[\"{DataAccessId}\"]"));
        Migration_01_02_00_Pre_00.TryConvertToEntries(element, Names);
        var entries = element.Stereotypes.Single().Properties.Single();
        var extra = new StereotypePersistable { DefinitionId = Ids.FileClassification.EntryDefinitionId, Properties = [] };
        extra.Properties.Add(new StereotypePropertyPersistable { DefinitionId = Ids.FileClassification.GlobId, Value = "Payments/**/*" });
        entries.ItemList.Add(extra);

        Migration_01_02_00_Pre_00.TryRevertToOutputClassification(element, Names).ShouldBeFalse();

        element.Stereotypes.ShouldHaveSingleItem().DefinitionId.ShouldBe(Ids.FileClassification.DefinitionId);
    }

    /// <summary>Must match the File Classification Entry stereotype's display function.</summary>
    [Theory]
    [InlineData("2 - Medium", new[] { "Data Access", "PCI scope" }, "**/*", "[Medium - Data Access, PCI scope] **/*")]
    [InlineData("3 - High", new string[0], "**/*.sql\n!seed.sql", "[High] **/*.sql, !seed.sql")]
    [InlineData(null, new[] { "Data Access" }, "**/*", "[Data Access] **/*")]
    [InlineData(null, new string[0], "**/*", "[] **/*")]
    [InlineData("1 - Low", new[] { "Data Access" }, "", "[Low - Data Access]")]
    public void Entry_label_matches_the_designer_display_format(string reviewImportance, string[] names, string glob, string expected)
    {
        Migration_01_02_00_Pre_00.FormatEntryLabel(reviewImportance, names, glob).ShouldBe(expected);
    }

    private static ElementPersistable NewTemplateOutput(params StereotypePersistable[] stereotypes)
    {
        var element = new ElementPersistable { SpecializationTypeId = Ids.TemplateOutputSpecializationTypeId, Name = "Some.Template" };
        element.Stereotypes.AddRange(stereotypes);
        return element;
    }

    private static StereotypePersistable WithOutputClassification(string reviewImportance, string classification)
    {
        var stereotype = new StereotypePersistable { DefinitionId = Ids.OutputClassification.DefinitionId, Name = Ids.OutputClassification.Name, Properties = [] };
        if (reviewImportance != null)
        {
            stereotype.Properties.Add(new StereotypePropertyPersistable { DefinitionId = Ids.OutputClassification.ReviewImportanceId, Value = reviewImportance });
        }

        stereotype.Properties.Add(new StereotypePropertyPersistable { DefinitionId = Ids.OutputClassification.ClassificationId, Value = classification });
        return stereotype;
    }

    private static string Value(StereotypePersistable stereotype, string definitionId) =>
        stereotype.Properties.Single(x => x.DefinitionId == definitionId).Value;
}
