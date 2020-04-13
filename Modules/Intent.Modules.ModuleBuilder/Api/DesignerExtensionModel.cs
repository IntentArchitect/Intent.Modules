using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DesignerExtensionModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Designer Extension";
        private readonly IElement _element;

        public DesignerExtensionModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'DesignerExtensionModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IList<ElementExtensionModel> ElementExtensions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementExtensionModel.SpecializationType)
            .Select(x => new ElementExtensionModel(x))
            .ToList<ElementExtensionModel>();
        [IntentManaged(Mode.Fully)]
        public IList<ElementSettingsModel> ElementTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettingsModel.SpecializationType)
            .Select(x => new ElementSettingsModel(x))
            .ToList<ElementSettingsModel>();
        [IntentManaged(Mode.Fully)]
        public IList<AssociationSettingsModel> AssociationTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AssociationSettingsModel.SpecializationType)
            .Select(x => new AssociationSettingsModel(x))
            .ToList<AssociationSettingsModel>();
        [IntentManaged(Mode.Fully)]
        public PackageSettingsModel PackageSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.PackageSettingsModel.SpecializationType)
            .Select(x => new PackageSettingsModel(x))
            .SingleOrDefault();

        protected bool Equals(DesignerExtensionModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DesignerExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}