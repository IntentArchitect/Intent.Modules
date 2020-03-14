using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    public class OperationSettings : IOperationSettings
    {
        private readonly IElement _element;
        public const string RequiredSpecializationType = "Operation Settings";

        public OperationSettings(IElement element)
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;
            SpecializationType = element.Name;
            Icon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full)"));
            Text = element.GetStereotypeProperty("Additional Properties", "Text", "New " + element.Name);
            Shortcut = element.GetStereotypeProperty("Additional Properties", "Shortcut", default(string));
            DefaultName = element.GetStereotypeProperty("Additional Properties", "Default Name", default(string));
            DisplayFunction = element.GetStereotypeProperty("Additional Properties", "Display Function", default(string));
            AllowRename = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Rename");
            AllowDuplicateNames = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Duplicate Names");
            AllowFindInView = element.GetStereotypeProperty<bool>("Additional Properties", "Allow Find in View");
            DefaultTypeId = element.GetStereotypeProperty("Additional Properties", "Default Type Id", default(string));
            TargetTypes = element.GetStereotypeProperty<IElement[]>("Additional Properties", "Target Types").Select(x => x.Name).ToArray();
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public string SpecializationType { get; set; }

        public IconModel Icon { get; set; }

        public string Text { get; set; }

        public string Shortcut { get; set; }

        public string DisplayFunction { get; set; }

        public string DefaultName { get; set; }

        public bool? AllowRename { get; set; }

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