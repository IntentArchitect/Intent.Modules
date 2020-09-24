using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using System;
using System.Linq;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Angular.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ServiceProxyModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Service Proxy";

        public ServiceProxyModel(IElement element)
        {
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;
        public string Comment => _element.Comment;
        public ModuleModel Module => new ModuleModel(_element.GetParentPath().Reverse().First(x => x.SpecializationType == ModuleModel.SpecializationType));

        [IntentManaged(Mode.Fully)]
        public bool IsMapped => _element.IsMapped;

        [IntentManaged(Mode.Fully)]
        public IElementMapping Mapping => _element.MappedElement;

        public ServiceModel MappedService => Mapping != null ? new ServiceModel((IElement)Mapping.Element) : null;

        [IntentManaged(Mode.Fully)]
        public IList<ServiceProxyOperationModel> Operations => _element.ChildElements
            .Where(x => x.SpecializationType == ServiceProxyOperationModel.SpecializationType)
            .Select(x => new ServiceProxyOperationModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(ServiceProxyModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceProxyModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }
        public const string SpecializationTypeId = "2d4ad132-e574-4319-9ae9-4d6e4b81110c";

        [IntentManaged(Mode.Ignore)]
        public ServiceProxyModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }
    }
}