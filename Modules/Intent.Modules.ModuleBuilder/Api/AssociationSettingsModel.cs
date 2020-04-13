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
    public class AssociationSettingsModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Association Settings";
        protected readonly IElement _element;

        public AssociationSettingsModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'AssociationSettingsModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public AssociationEndSettingsModel SourceEnd => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AssociationEndSettingsModel.SpecializationType)
            .Select(x => new AssociationEndSettingsModel(x))
            .SingleOrDefault();
        [IntentManaged(Mode.Fully)]
        public AssociationEndSettingsModel DestinationEnd => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AssociationEndSettingsModel.SpecializationType)
            .Select(x => new AssociationEndSettingsModel(x))
            .SingleOrDefault();

        protected bool Equals(AssociationSettingsModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }
}