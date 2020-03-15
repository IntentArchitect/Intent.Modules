using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class ModelerExtension : IModelerExtension
    {
        public const string SpecializationType = "Modeler Extension";
        private readonly IElement _element;

        public ModelerExtension(IElement element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new ArgumentException($"Invalid element type {element.SpecializationType}", nameof(element));
            }
            _element = element;
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IList<IAssociationSettings> AssociationTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AssociationSettings.SpecializationType)
            .Select(x => new AssociationSettings(x))
            .ToList<IAssociationSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IElementSettings> ElementTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettings.SpecializationType)
            .Select(x => new ElementSettings(x))
            .ToList<IElementSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<IPackageSettings> PackageSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.PackageSettings.SpecializationType)
            .Select(x => new PackageSettings(x))
            .ToList<IPackageSettings>();

        protected bool Equals(ModelerExtension other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelerExtension)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}