using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api.Modeler
{
    public class LiteralSetting
    {
        public const string RequiredSpecializationType = "Literal Settings";
        public LiteralSetting(IElement element)
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            SpecializationType = element.Name;
            Icon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full)"));
            Text = element.GetStereotypeProperty("Additional Properties", "Text", "New " + element.Name);
            Shortcut = element.GetStereotypeProperty("Additional Properties", "Shortcut", default(string));
            DefaultName = element.GetStereotypeProperty("Additional Properties", "Default Name", default(string));
            AllowRename = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
            AllowDuplicateNames = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Duplicate Names");
            AllowFindInView = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
        }

        public string SpecializationType { get; set; }

        public IconModel Icon { get; set; }

        public string Text { get; set; }

        public string Shortcut { get; set; }

        public string DefaultName { get; set; }

        public bool? AllowRename { get; set; } = true;

        public bool? AllowDuplicateNames { get; set; }

        public bool? AllowFindInView { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }
}