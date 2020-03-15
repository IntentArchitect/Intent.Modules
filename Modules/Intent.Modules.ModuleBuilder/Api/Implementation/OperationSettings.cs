using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class OperationSettings : IOperationSettings
    {
        private readonly IElement _element;
        public const string SpecializationType = "Operation Settings";

        public OperationSettings(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;
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
            return $"{nameof(Name)} = '{Name}', " +
                   $"{nameof(Text)} = '{Text}'";
        }

        protected bool Equals(OperationSettings other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OperationSettings)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}