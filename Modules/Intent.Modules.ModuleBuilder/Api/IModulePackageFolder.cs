using System;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModulePackageFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IElement Parent { get; }
    }

    class ModulePackageFolder : IModulePackageFolder
    {
        public const string SpecializationType = "Module Package Folder";
        private readonly IElement _element;

        public ModulePackageFolder(IElement element)
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
        public IElement Parent => _element.ParentElement;
    }
}