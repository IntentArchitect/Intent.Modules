using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Services.EventInteractions
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MessageModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Message";
        public const string SpecializationTypeId = "cbe970af-5bad-4d92-a3ed-a24b9fdaa23e";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MessageModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public IList<PropertyModel> Properties => _element.ChildElements
            .GetElementsOfType(PropertyModel.SpecializationTypeId)
            .Select(x => new PropertyModel(x))
            .ToList();

        public IList<DTOFieldModel> Fields => _element.ChildElements
            .GetElementsOfType(DTOFieldModel.SpecializationTypeId)
            .Select(x => new DTOFieldModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MessageModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MessageModelExtensions
    {

        public static bool IsMessageModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MessageModel.SpecializationTypeId;
        }

        public static MessageModel AsMessageModel(this ICanBeReferencedType type)
        {
            return type.IsMessageModel() ? new MessageModel((IElement)type) : null;
        }
    }
}