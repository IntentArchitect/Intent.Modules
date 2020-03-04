using System;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api.Modeler
{
    public class AttributeSetting
    {
        public const string RequiredSpecializationType = "Attribute Settings";

        public AttributeSetting(IElement element)
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }
        }

        public string SpecializationType { get; set; }

        public IconModel Icon { get; set; }

        public string Text { get; set; }

        public string Shortcut { get; set; }

        public string DisplayFunction { get; set; }

        public string DefaultName { get; set; }

        public bool? AllowRename { get; set; } = true;

        public bool? AllowDuplicateNames { get; set; }

        public bool? AllowFindInView { get; set; }

        public string DefaultTypeId { get; set; }

        public string[] TargetTypes { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }
}