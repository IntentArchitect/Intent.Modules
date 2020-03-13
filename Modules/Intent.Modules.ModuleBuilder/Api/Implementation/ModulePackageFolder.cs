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
    internal class ModulePackageFolder : IModulePackageFolder
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
        public string FolderPath => Path.Combine(_element.GetParentPath()
            .Reverse()
            .TakeWhile(x => x.SpecializationType != "Metadata Folder")
            .Reverse()
            .Select(x => x.Name).ToArray());
        
    }
}