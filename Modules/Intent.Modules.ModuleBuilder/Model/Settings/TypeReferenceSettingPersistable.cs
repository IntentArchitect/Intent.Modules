using System;
using System.Linq;
using System.Xml.Serialization;

namespace Intent.IArchitect.Agent.Persistence.Model.Common;

public class TypeReferenceSettingPersistable
{
    [XmlElement("displayName")]
    public string DisplayName { get; set; }

    [XmlElement("hint")]
    public string Hint { get; set; }

    [XmlElement("isRequired")]
    public bool IsRequired { get; set; } = true;

    [XmlArray("targetTypes")]
    [XmlArrayItem("type")]
    public TargetTypePersistable[] TargetTypes { get; set; } = Array.Empty<TargetTypePersistable>();

    [XmlElement("defaultTypeId")]
    public string DefaultTypeId { get; set; }

    [XmlElement("allowIsNavigable")]
    public bool AllowIsNavigable { get; set; } = true;

    [XmlElement("allowIsNullable")]
    public bool AllowIsNullable { get; set; } = true;

    [XmlElement("allowIsCollection")]
    public bool AllowIsCollection { get; set; } = true;

    [XmlElement("isNavigableDefault")]
    public bool IsNavigableDefault { get; set; } = true;

    [XmlElement("isNullableDefault")]
    public bool IsNullableDefault { get; set; }

    [XmlElement("isCollectionDefault")]
    public bool IsCollectionDefault { get; set; }

    private bool _hasBeenAdjusted = false;
    public void AdjustFromExtension(TypeReferenceExtensionSettingPersistable extension)
    {
        var isGettingAdjusted = false;
        if (IsRequired != extension.IsRequired)
        {
            IsRequired = extension.IsRequired;
            isGettingAdjusted = true;
        }

        if (!string.IsNullOrWhiteSpace(extension.DisplayName))
        {
            DisplayName = extension.DisplayName;
            isGettingAdjusted = true;
        }

        if (!string.IsNullOrWhiteSpace(extension.Hint))
        {
            Hint = extension.Hint;
            isGettingAdjusted = true;
        }

        if (extension.TargetTypes != null && extension.TargetTypes.Any())
        {
            // Extend target types
            TargetTypes = TargetTypes.Concat(extension.TargetTypes).Distinct().ToArray();
            //isGettingAdjusted = true; (multiple extensions can extend these target types).
        }

        DefaultTypeId = extension.DefaultTypeId ?? DefaultTypeId;
        if (extension.AllowIsNavigable != BooleanExtensionOptions.Inherit)
        {
            AllowIsNavigable = extension.AllowIsNavigable == BooleanExtensionOptions.True;
            isGettingAdjusted = true;
        }
        if (extension.AllowIsNullable != BooleanExtensionOptions.Inherit)
        {
            AllowIsNullable = extension.AllowIsNullable == BooleanExtensionOptions.True;
            isGettingAdjusted = true;
        }
        if (extension.AllowIsCollection != BooleanExtensionOptions.Inherit)
        {
            AllowIsCollection = extension.AllowIsCollection == BooleanExtensionOptions.True;
            isGettingAdjusted = true;
        }

        if (isGettingAdjusted && _hasBeenAdjusted)
        {
            throw new InvalidOperationException("Type Reference Settings have already been adjusted by a previous extension. " +
                                                "To ensure that designer settings are predictable, more than one " +
                                                "extension cannot change these values.");
        }

        if (isGettingAdjusted)
        {
            _hasBeenAdjusted = true;
        }
    }
}