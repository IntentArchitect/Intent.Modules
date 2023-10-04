using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Domain.Events.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DomainEventHandlerModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Domain Event Handler";
        public const string SpecializationTypeId = "d80e61c5-7e4c-4175-9df1-0413f664824c";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DomainEventHandlerModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DomainEventHandlerModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainEventHandlerModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DomainEventHandlerModelExtensions
    {

        public static bool IsDomainEventHandlerModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DomainEventHandlerModel.SpecializationTypeId;
        }

        public static DomainEventHandlerModel AsDomainEventHandlerModel(this ICanBeReferencedType type)
        {
            return type.IsDomainEventHandlerModel() ? new DomainEventHandlerModel((IElement)type) : null;
        }
    }
}