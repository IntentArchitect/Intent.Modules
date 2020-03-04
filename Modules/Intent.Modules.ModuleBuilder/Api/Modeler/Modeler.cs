using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.IArchitect.Common.Types;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api.Modeler
{
    public class Modeler
    {
        public const string RequiredSpecializationType = "Modeler";

        public Modeler(IElement element)
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            Name = element.Name;
            PackageSettings = new PackageSettings(element.ChildElements.SingleOrDefault(x => x.SpecializationType == PackageSettings.SpecializationType));
            ElementSettings = element.ChildElements.Where(x => x.SpecializationType == ElementSetting.RequiredSpecializationType).Select(x => new ElementSetting(x)).ToList();
        }

        public PackageSettings PackageSettings { get; }
        public IList<ElementSetting> ElementSettings { get; }
        public string Name { get; }
    }

    public class TypeOrder
    {
        public int? Order { get; set; }
        public string Type { get; set; }
    }

    public class CreationOption
    {
        public const string RequiredSpecializationType = "Creation Option";

        public CreationOption(IAttribute attribute)
        {
            if (attribute.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{attribute}]");
            }

            SpecializationType = attribute.Type.Element.SpecializationType;
            Text = attribute.Name;
            Shortcut = attribute.Type.Element.GetStereotypeProperty<string>("Default Creation Options", "Shortcut");
            DefaultName = attribute.Type.Element.GetStereotypeProperty<string>("Default Creation Options", "Default Name") ?? $"New{attribute.Type.Element.Name.Replace(" " ,"")}";
            Icon = attribute.Type.Element.HasStereotype("Icon (Full)") ? new IconModel(attribute.Type.Element.GetStereotype("Icon (Full)")) : null;
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

    public class IconModel
    {
        public IconModel(IStereotype stereotype)
        {
            Type = (IconType) Enum.Parse(typeof(IconType), stereotype.GetProperty<string>("Type"));
            Source = stereotype.GetProperty<string>("Source");
        }

        public IconType Type { get; set; }
        public string Source { get; set; }
    }

    public class OperationSetting
    {

    }
}
