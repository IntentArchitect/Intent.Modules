using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleStereotype : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string ParentFolderId { get; }
        bool DisplayIcon { get; }
        bool AutoAdd { get; }
    }

    class ModuleStereotype : IModuleStereotype
    {
        public const string SpecializationType = "Module Stereotype";
        private readonly IElement _element;

        public ModuleStereotype(IElement element)
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
        public string ParentFolderId => _element.ParentElement.Id;
        public bool DisplayIcon => _element.GetStereotypeProperty("Module Stereotype Settings", "Display Icon", false);
        public bool AutoAdd => _element.GetStereotypeProperty("Module Stereotype Settings", "Auto Add", false);
    }
}