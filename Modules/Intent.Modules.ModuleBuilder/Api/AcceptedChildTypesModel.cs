using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AcceptedChildTypesModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Accepted Child Types";
        public const string SpecializationTypeId = "7c0e37ab-2f84-46df-ac29-1e5aaa67bf75";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AcceptedChildTypesModel(IElement element, string requiredType = SpecializationTypeId)
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

        public IList<AcceptedTypeModel> AcceptedTypes => _element.ChildElements
            .GetElementsOfType(AcceptedTypeModel.SpecializationTypeId)
            .Select(x => new AcceptedTypeModel(x))
            .ToList();

        public IList<AcceptedTraitsModel> AcceptedTraits => _element.ChildElements
            .GetElementsOfType(AcceptedTraitsModel.SpecializationTypeId)
            .Select(x => new AcceptedTraitsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AcceptedChildTypesModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AcceptedChildTypesModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public List<TypeOrderPersistable> ToPersistable()
        {
            var order = 0;
            var result = new List<TypeOrderPersistable>();
            foreach (var childElement in InternalElement.ChildElements)
            {
                if (childElement.IsAcceptedTypeModel())
                {
                    var model = childElement.AsAcceptedTypeModel();
                    result.Add(new TypeOrderPersistable() { Order = order, Type = model.TypeReference.Element.Name, TypeId = model.TypeReference.Element.Id });
                }
                if (childElement.IsAcceptedTraitsModel())
                {
                    var model = childElement.AsAcceptedTraitsModel();
                    if (!model.GetSettings().TypesWithStereotype().IsTrait)
                    {
                        throw new ElementException(childElement, $"Selected Stereotype Definition '{model.GetSettings().TypesWithStereotype().Name}' is not marked as a trait. Select another Stereotype Definition or mark the selected one as `'Is Trait' = true`");
                    }
                    result.Add(new TypeOrderPersistable() { Order = order, Type = model.GetSettings().TypesWithStereotype().Name, TypeId = model.GetSettings().TypesWithStereotype().Id });
                }
                order++;
            }
            return result;
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AcceptedChildTypesModelExtensions
    {

        public static bool IsAcceptedChildTypesModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AcceptedChildTypesModel.SpecializationTypeId;
        }

        public static AcceptedChildTypesModel AsAcceptedChildTypesModel(this ICanBeReferencedType type)
        {
            return type.IsAcceptedChildTypesModel() ? new AcceptedChildTypesModel((IElement)type) : null;
        }
    }
}