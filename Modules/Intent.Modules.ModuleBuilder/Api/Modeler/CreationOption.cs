using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api.Modeler
{
    public class CreationOption
    {
        public const string RequiredSpecializationType = "Creation Option";

        public CreationOption(IAttribute attribute)
        {
            if (attribute.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{attribute}]");
            }

            SpecializationType = attribute.Type.Element.Name;
            Text = attribute.Name;
            Shortcut = attribute.Type.Element.GetStereotypeProperty<string>("Default Creation Options", "Shortcut");
            DefaultName = attribute.Type.Element.GetStereotypeProperty<string>("Default Creation Options", "Default Name") ?? $"New{attribute.Type.Element.Name.Replace(" " ,"")}";
            Icon = IconModel.CreateIfSpecified(attribute.Type.Element.GetStereotype("Icon (Full)"));
        }

        public string SpecializationType { get; }

        public string Text { get; }

        public string Shortcut { get; }

        public string DefaultName { get; }

        public IconModel Icon { get; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }
}