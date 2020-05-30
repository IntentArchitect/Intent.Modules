using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class AssociationSourceEndSettingsModel : IHasStereotypes, IMetadataModel, ICreatableType
    {
        public const string SpecializationType = "Association Source End Settings";
        protected readonly IElement _element;

        public AssociationSourceEndSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        public string ApiModelName => $"{_element.ParentElement.Name.ToCSharpIdentifier()}EndModel";

        public string ApiPropertyName => this.GetSettings().ApiPropertyName();

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public bool TargetsType(string elementSettingsId)
        {
            return this.GetSettings().TargetTypes().Any(t => t.Id == elementSettingsId);
        }

        public List<ElementSettingsModel> TargetTypes()
        {
            return this.GetSettings().TargetTypes().Select(x => new ElementSettingsModel(x)).ToList();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationSourceEndSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationSourceEndSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
    }
}