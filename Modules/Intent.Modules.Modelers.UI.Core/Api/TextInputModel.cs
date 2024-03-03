using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class TextInputModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Text Input";
        public const string SpecializationTypeId = "4803bf60-c626-4cbe-94ed-0f1eafff9fe3";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public TextInputModel(IElement element, string requiredType = SpecializationType)
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

        public IList<EventEmitterModel> EventEmitters => _element.ChildElements
            .GetElementsOfType(EventEmitterModel.SpecializationTypeId)
            .Select(x => new EventEmitterModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(TextInputModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TextInputModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class TextInputModelExtensions
    {

        public static bool IsTextInputModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == TextInputModel.SpecializationTypeId;
        }

        public static TextInputModel AsTextInputModel(this ICanBeReferencedType type)
        {
            return type.IsTextInputModel() ? new TextInputModel((IElement)type) : null;
        }
    }
}