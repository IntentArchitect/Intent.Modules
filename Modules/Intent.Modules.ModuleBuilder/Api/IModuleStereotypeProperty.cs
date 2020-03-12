using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleStereotypeProperty : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string ControlType { get; }
        string OptionsSource { get; }
        string Comment { get; }
        IList<string> ValueOptions { get; }
        IList<string> LookupTypes { get; }
        string TargetPropertyId { get; }
        string DefaultValue { get; }
        string IsActiveFunction { get; }
    }

    class ModuleStereotypeProperty : IModuleStereotypeProperty
    {
        public const string SpecializationType = "Module Stereotype Property";
        private readonly IElement _element;

        public ModuleStereotypeProperty(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
            }
            _element = element; 
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name; 
        public string ControlType
        {
            get
            {
                switch (_element.GetStereotypeProperty("Module Stereotype Property Settings", "Control Type", "Text Box"))
                {
                    case "Text Box":
                        return "text";
                    case "Number":
                        return "number";
                    case "Checkbox":
                        return "checkbox";
                    case "Text Area":
                        return "textarea";
                    case "Select":
                        return "select";
                    case "Multi-select":
                        return "multi-select";
                }

                return "text";
            }
        }

        public string OptionsSource => _element.GetStereotypeProperty("Module Stereotype Property Settings", "Options Source", "N/A").ToLower().Replace(" ", "-");
        public IList<string> ValueOptions => _element.Literals.Select(x => x.Name).ToList();
        public IList<string> LookupTypes => _element.GetStereotypeProperty<IElement[]>("Module Stereotype Property Settings", "Lookup Types", new IElement[0])
            .Select(x => x.Name).ToList();
        public string TargetPropertyId => _element.GetStereotypeProperty("Module Stereotype Property Settings", "Target Property", string.Empty);
        public string DefaultValue => _element.GetStereotypeProperty("Module Stereotype Property Settings", "Default Value", string.Empty);
        public string IsActiveFunction => _element.GetStereotypeProperty("Module Stereotype Property Settings", "Is Active Function", string.Empty);
        public string Comment => _element.Comment; 
    }
}