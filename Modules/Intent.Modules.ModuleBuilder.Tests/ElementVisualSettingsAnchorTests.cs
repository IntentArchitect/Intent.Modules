extern alias mb;

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Intent.Metadata.Models;
using mb::Intent.IArchitect.Agent.Persistence.Model.Common;
using mb::Intent.ModuleBuilder.Api;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.ModuleBuilder.Tests;

public class ElementVisualSettingsAnchorTests
{
    [Fact]
    public void Anchor_segments_and_even_spread_are_serialised()
    {
        var xml = Serialize(NewModel(anchorSegments: "return [[{x: 0, y: 0}, {x: 10, y: 0}]];", anchorSpread: "Even").ToPersistable());

        xml.ShouldContain("<anchorSegmentsFunction>return [[{x: 0, y: 0}, {x: 10, y: 0}]];</anchorSegmentsFunction>");
        xml.ShouldContain("<anchorSpread>even</anchorSpread>");
    }

    [Fact]
    public void Off_spread_and_empty_segments_emit_nothing()
    {
        var xml = Serialize(NewModel(anchorSegments: null, anchorSpread: "Off").ToPersistable());

        xml.ShouldNotContain("anchorSpread");
        xml.ShouldNotContain("anchorSegmentsFunction");
    }

    [Fact]
    public void Existing_anchor_points_function_still_round_trips()
    {
        var xml = Serialize(new ElementVisualSettingsPersistable { AnchorPointsFunction = "return [];" });

        xml.ShouldContain("<anchorPointsFunction>return [];</anchorPointsFunction>");
        var read = (ElementVisualSettingsPersistable)new XmlSerializer(typeof(ElementVisualSettingsPersistable)).Deserialize(new StringReader(xml))!;
        read.AnchorPointsFunction.ShouldBe("return [];");
    }

    private static string Serialize(ElementVisualSettingsPersistable persistable)
    {
        using var writer = new StringWriter();
        new XmlSerializer(typeof(ElementVisualSettingsPersistable)).Serialize(writer, persistable);
        return writer.ToString();
    }

    private static ElementVisualSettingsModel NewModel(string anchorSegments, string anchorSpread)
    {
        // Substitutes are built before any Returns() call, because NSubstitute can't nest them.
        var referenced = Substitute.For<ICanBeReferencedType>();
        var typeReference = Substitute.For<ITypeReference>();
        typeReference.Element.Returns(referenced);
        var stereotypes = new List<IStereotype>
        {
            Stereotype(ElementVisualSettingsModelStereotypeExtensions.Settings.DefinitionId,
                ("Anchor Segments", anchorSegments), ("Anchor Spread", anchorSpread)),
            Stereotype(ElementVisualSettingsModelStereotypeExtensions.PositionSettings.DefinitionId)
        };

        var element = Substitute.For<IElement>();
        element.SpecializationType.Returns(ElementVisualSettingsModel.SpecializationType);
        element.TypeReference.Returns(typeReference);
        element.Stereotypes.Returns(stereotypes);
        return new ElementVisualSettingsModel(element);
    }

    private static IStereotype Stereotype(string definitionId, params (string Name, string Value)[] properties)
    {
        var stereotype = Substitute.For<IStereotype>();
        stereotype.DefinitionId.Returns(definitionId);
        stereotype.TryGetProperty(Arg.Any<string>(), out Arg.Any<IStereotypeProperty>()).Returns(call =>
        {
            foreach (var (name, value) in properties)
            {
                if (name == call.Arg<string>() && value != null)
                {
                    var property = Substitute.For<IStereotypeProperty>();
                    property.Value.Returns(value);
                    call[1] = property;
                    return true;
                }
            }

            call[1] = null;
            return false;
        });
        return stereotype;
    }
}
