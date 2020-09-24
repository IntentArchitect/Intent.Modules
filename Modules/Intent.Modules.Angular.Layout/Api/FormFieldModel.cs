using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class FormFieldModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Form Field";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public FormFieldModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            if (InternalElement.IsMapped)
            {
                DataModelPath = string.Join(".", InternalElement.MappedElement.Path.Select(x => x.Name.ToCamelCase()));
            }
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        public string DataModelPath { get; } = "";

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(FormFieldModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FormFieldModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        public const string SpecializationTypeId = "1e3e3916-0cdb-45bb-a3b8-d7e01834cc36";
    }
}