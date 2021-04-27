using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Types.ServiceProxies.Api
{
    [IntentManaged(Mode.Merge)]
    public class OperationModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "Operation";
        public const string SpecializationTypeId = "aee6811e-b2f6-4562-a8eb-502029f63bc8";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public OperationModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        [IntentManaged(Mode.Ignore)]
        public ITypeReference ReturnType => TypeReference.Element != null ? TypeReference : null;

        [IntentManaged(Mode.Ignore)]
        public bool IsMapped => _element.IsMapped;

        [IntentManaged(Mode.Ignore)]
        public IElementMapping Mapping => _element.MappedElement;

        [IntentManaged(Mode.Ignore)]
        public ServiceProxyModel ParentService => new ServiceProxyModel(InternalElement.ParentElement);

        [IntentManaged(Mode.Ignore)]
        public Services.Api.OperationModel MappedOperation => Mapping != null ? new Services.Api.OperationModel((IElement)Mapping.Element) : null;

        public IList<ParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(ParameterModel.SpecializationTypeId)
            .Select(x => new ParameterModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(OperationModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OperationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;
    }
}