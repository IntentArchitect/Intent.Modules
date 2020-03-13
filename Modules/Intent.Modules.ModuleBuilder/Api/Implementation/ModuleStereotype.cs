using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class ModuleStereotype : IModuleStereotype
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
            Properties = _element.ChildElements.Select(x => new ModuleStereotypeProperty(x)).ToList<IModuleStereotypeProperty>();
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;
        public IEnumerable<string> TargetTypes => _element.GetStereotypeProperty("Module Stereotype Settings", "Targets", new IElement[0]).Select(x => x.Name);
        public string ParentFolderId => _element.ParentElement.Id;
        public bool DisplayIcon => _element.GetStereotypeProperty("Module Stereotype Settings", "Display Icon", false);
        public string DisplayFunction => _element.GetStereotypeProperty("Module Stereotype Settings", "Display Text", string.Empty);
        public bool AutoAdd => _element.GetStereotypeProperty("Module Stereotype Settings", "Auto Add", false);
        public IElement Parent => _element.ParentElement;
        public IList<IModuleStereotypeProperty> Properties { get; }
        public IModulePackage GetPackage()
        {
            return new ModulePackage(GetParentPath(_element).Single(x => x.SpecializationType == ModulePackage.SpecializationType));
        }

        public string FolderPath => Path.Combine(GetParentPath(_element)
            .Reverse()
            .TakeWhile(x => x.SpecializationType != "Metadata Folder")
            .Reverse()
            .Select(x => x.Name).ToArray());

        private static IList<IElement> GetParentPath(IElement model)
        {
            List<IElement> result = new List<IElement>();

            var current = model.ParentElement;
            while (current != null)
            {
                result.Insert(0, current);
                current = current.ParentElement;
            }
            return result;
        }
    }
}