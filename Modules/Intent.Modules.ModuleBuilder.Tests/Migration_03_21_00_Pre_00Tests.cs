extern alias mb;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using mb::Intent.ModuleBuilder.Api;
using mb::Intent.Modules.ModuleBuilder.Migrations;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Ids = mb::Intent.Modules.ModuleBuilder.Migrations.Migration_03_21_00_Pre_00.Ids;

namespace Intent.Modules.ModuleBuilder.Tests;

public class Migration_03_21_00_Pre_00Tests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    /// <summary>
    /// The migration reads these from consumers' metadata, so a typo silently leaves Anchor Points behind.
    /// They must equal this module's own Module Builder ids.
    /// </summary>
    [Fact]
    public void Ids_match_the_module_metadata()
    {
        Ids.SettingsDefinitionId.ShouldBe("96a4fa17-6283-4626-835e-a8f013d98b19");
        Ids.AnchorPointsId.ShouldBe("838dcc53-b73e-4381-a3d2-db0afaf230b2");
        Ids.AnchorSegmentsId.ShouldBe("c5c75dd7-f419-4a03-8b78-e395ede92ddd");
    }

    [Fact]
    public void Anchor_points_become_segments_and_the_old_value_is_cleared()
    {
        var element = NewVisualSettings(("Anchor Points", Ids.AnchorPointsId, "return [{x: 0, y: 0}, {x: 10, y: 5}];"));

        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(element).ShouldBeTrue();

        Value(element, Ids.AnchorPointsId).ShouldBeNull();
        Value(element, Ids.AnchorSegmentsId).ShouldBe(
            "const points = (() => { return [{x: 0, y: 0}, {x: 10, y: 5}]; })(); return points.map(p => [p, p]);");
    }

    [Theory]
    [InlineData("const w = getDimensions().width; return [{x: w, y: 0}];")]
    [InlineData("let p = []; return p;")]
    [InlineData("var p = []; return p;")]
    [InlineData("if (true) { return []; } return [];")]
    [InlineData("function f() { return []; } return f();")]
    public void Every_accepted_body_start_is_converted(string body)
    {
        var element = NewVisualSettings(("Anchor Points", Ids.AnchorPointsId, body));

        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(element).ShouldBeTrue();

        Value(element, Ids.AnchorSegmentsId).ShouldContain(body);
    }

    [Fact]
    public void A_body_that_never_returned_an_array_is_left_alone()
    {
        var element = NewVisualSettings(("Anchor Points", Ids.AnchorPointsId, "[{x: 0, y: 0}]"));

        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(element).ShouldBeFalse();

        Value(element, Ids.AnchorPointsId).ShouldBe("[{x: 0, y: 0}]");
        Value(element, Ids.AnchorSegmentsId).ShouldBeNull();
    }

    [Fact]
    public void Existing_anchor_segments_are_kept_and_the_old_value_is_cleared()
    {
        var element = NewVisualSettings(
            ("Anchor Points", Ids.AnchorPointsId, "return [];"),
            ("Anchor Segments", Ids.AnchorSegmentsId, "return [[{x: 1, y: 1}, {x: 2, y: 1}]];"));

        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(element).ShouldBeTrue();

        Value(element, Ids.AnchorPointsId).ShouldBeNull();
        Value(element, Ids.AnchorSegmentsId).ShouldBe("return [[{x: 1, y: 1}, {x: 2, y: 1}]];");
    }

    [Fact]
    public void Nothing_changes_without_an_anchor_points_value()
    {
        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(NewVisualSettings(("Anchor Points", Ids.AnchorPointsId, null))).ShouldBeFalse();
        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(NewVisualSettings()).ShouldBeFalse();
        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(new ElementPersistable()).ShouldBeFalse();
    }

    /// <summary>
    /// The element as a consumer's package stores it (shape taken from a real Element Visual Settings, with an
    /// Anchor Points value added), so this also proves the property ids line up with the persisted attributes.
    /// </summary>
    [Fact]
    public void Persisted_element_is_converted_and_written_back_without_anchor_points()
    {
        const string persisted = """
            <?xml version="1.0" encoding="utf-8"?>
            <class id="2a80d0c8-e121-42b9-b55c-3b5ec8294b09" type="Element Visual Settings" typeId="5eea5fba-15df-4971-8541-6f19bad4297d">
              <name>NewElementVisualSettings</name>
              <isAbstract>false</isAbstract>
              <genericTypes />
              <isMapped>false</isMapped>
              <stereotypes>
                <stereotype stereotypeDefinitionId="96a4fa17-6283-4626-835e-a8f013d98b19" name="Settings">
                  <addedByDefault>true</addedByDefault>
                  <definitionPackageName>Intent.ModuleBuilder</definitionPackageName>
                  <definitionPackageId>9972b2a9-b749-4bba-b5c8-824bf694c6ef</definitionPackageId>
                  <properties>
                    <property name="838dcc53-b73e-4381-a3d2-db0afaf230b2" display="Anchor Points" value="return [{x: getDimensions().x, y: getDimensions().y}];" isActive="true" />
                    <property name="43c4e468-1139-42b3-a064-fb49f679b72b" display="Auto-Resize Default" value="true" isActive="true" />
                  </properties>
                </stereotype>
              </stereotypes>
              <metadata />
              <childElements />
            </class>
            """;
        var serializer = new XmlSerializer(typeof(ElementPersistable));
        var element = (ElementPersistable)serializer.Deserialize(new StringReader(persisted))!;

        Migration_03_21_00_Pre_00.TryConvertAnchorPoints(element).ShouldBeTrue();

        using var writer = new StringWriter();
        serializer.Serialize(writer, element);
        var after = writer.ToString();
        _output.WriteLine(after);
        after.ShouldContain("display=\"Anchor Segments\"");
        after.ShouldContain("const points = (() =&gt; { return [{x: getDimensions().x, y: getDimensions().y}]; })(); return points.map(p =&gt; [p, p]);");
        after.ShouldNotContain("return [{x: getDimensions().x, y: getDimensions().y}];\" isActive");
        element.Stereotypes.Single().Properties.Single(x => x.Id == Ids.AnchorPointsId).Value.ShouldBeNull();
        element.Stereotypes.Single().Properties.Single(x => x.Id == "43c4e468-1139-42b3-a064-fb49f679b72b").Value.ShouldBe("true");
    }

    private static ElementPersistable NewVisualSettings(params (string Name, string Id, string Value)[] properties)
    {
        var settings = new StereotypePersistable
        {
            DefinitionId = Ids.SettingsDefinitionId,
            Name = "Settings",
            Properties = new List<StereotypePropertyPersistable>()
        };
        foreach (var (name, id, value) in properties)
        {
            settings.Properties.Add(new StereotypePropertyPersistable { Id = id, Name = name, Value = value });
        }

        var element = new ElementPersistable
        {
            SpecializationTypeId = ElementVisualSettingsModel.SpecializationTypeId,
            Name = "Some Visual"
        };
        element.Stereotypes.Add(settings);
        return element;
    }

    private static string Value(ElementPersistable element, string propertyId) =>
        element.Stereotypes.Single().Properties.FirstOrDefault(x => x.Id == propertyId)?.Value;
}
