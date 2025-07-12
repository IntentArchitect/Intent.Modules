using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.UI.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CardModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
    {
        public const string SpecializationType = "Card";
        public const string SpecializationTypeId = "dfe420aa-426a-4517-bcd1-83cf5ec074fe";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public CardModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
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

        public CardHeaderModel Header => _element.ChildElements
            .GetElementsOfType(CardHeaderModel.SpecializationTypeId)
            .Select(x => new CardHeaderModel(x))
            .SingleOrDefault();

        public CardContentModel Content => _element.ChildElements
            .GetElementsOfType(CardContentModel.SpecializationTypeId)
            .Select(x => new CardContentModel(x))
            .SingleOrDefault();

        public CardActionsModel Actions => _element.ChildElements
            .GetElementsOfType(CardActionsModel.SpecializationTypeId)
            .Select(x => new CardActionsModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CardModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CardModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CardModelExtensions
    {

        public static bool IsCardModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CardModel.SpecializationTypeId;
        }

        public static CardModel AsCardModel(this ICanBeReferencedType type)
        {
            return type.IsCardModel() ? new CardModel((IElement)type) : null;
        }
    }
}