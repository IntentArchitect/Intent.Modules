using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public class PackageSettings : IPackageSettings
    {
        private readonly IElement _element;
        public const string SpecializationType = "Package Settings";

        public PackageSettings(IElement element)
        {
            if (element?.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;

            ContextMenu = element.ChildElements.Any(x => x.SpecializationType == Api.ContextMenu.SpecializationType) 
                ? new ContextMenu(element.ChildElements.Single(x => x.SpecializationType == Api.ContextMenu.SpecializationType)) 
                : null;
        }

        public static IPackageSettings Create(IElement element)
        {
            return element != null ? new PackageSettings(element) : null;
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;
        public IContextMenu ContextMenu { get; }


    }
}