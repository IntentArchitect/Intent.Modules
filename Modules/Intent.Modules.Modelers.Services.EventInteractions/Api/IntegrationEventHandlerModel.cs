using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class IntegrationEventHandlerModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Integration Event Handler";
        public const string SpecializationTypeId = "c0582230-22f5-4f74-8eb0-ec6fc9364900";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IntegrationEventHandlerModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(IntegrationEventHandlerModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegrationEventHandlerModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IntegrationEventHandlerModelExtensions
    {

        public static bool IsIntegrationEventHandlerModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == IntegrationEventHandlerModel.SpecializationTypeId;
        }

        public static IntegrationEventHandlerModel AsIntegrationEventHandlerModel(this ICanBeReferencedType type)
        {
            return type.IsIntegrationEventHandlerModel() ? new IntegrationEventHandlerModel((IElement)type) : null;
        }
    }
}