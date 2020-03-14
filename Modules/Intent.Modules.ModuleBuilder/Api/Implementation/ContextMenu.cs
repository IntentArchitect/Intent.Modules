using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class ContextMenu : IContextMenu
    {
        public const string SpecializationType = "Context Menu";
        private readonly IElement _element;

        public ContextMenu(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
            }
            _element = element;
            CreationOptions = element.ChildElements.Select(x => new CreationOption(x)).ToList<ICreationOption>();
            TypeOrder = element.ChildElements.Select(x => new TypeOrder(x)).ToList();
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public IList<ICreationOption> CreationOptions { get; }
        public IList<TypeOrder> TypeOrder { get; }
    }
}