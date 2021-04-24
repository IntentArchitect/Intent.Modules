using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationDestinationEndSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Association Destination End Settings";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public AssociationDestinationEndSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public string ApiModelName => $"{Name.ToCSharpIdentifier()}Model";

        public string ApiPropertyName => this.GetSettings().ApiPropertyName();

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationDestinationEndSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationDestinationEndSettingsModel)obj);
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
        public const string SpecializationTypeId = "c4c61fdc-464d-41d2-8e0e-5a734d588302";

        public string Comment => _element.Comment;
    }
}