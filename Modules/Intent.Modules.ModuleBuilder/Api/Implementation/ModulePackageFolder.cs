using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
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