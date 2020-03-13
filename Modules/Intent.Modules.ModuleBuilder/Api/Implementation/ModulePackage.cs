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
    internal class ModulePackage : IModulePackage
    {
        public const string SpecializationType = "Module Package";
        private readonly IElement _element;

        public ModulePackage(IElement element)
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
        public string IconUrl => "./img/icons/common/Intent-Package-Dark.png";

        public List<string> TargetModelers => _element
            .GetStereotypeProperty<IElement[]>("Module Package Settings", "Target Modelers", new IElement[0])
            .Select(x => x.Name).ToList();
    }
}