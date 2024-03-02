using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class EventEmitterModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IHasTypeReference
    {
        public const string SpecializationType = "Event Emitter";
        public const string SpecializationTypeId = "d6739ffc-30e6-4170-a105-bf28e69aa578";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public EventEmitterModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;


        public IElement InternalElement => _element;

        public IList<ParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(ParameterModel.SpecializationTypeId)
            .Select(x => new ParameterModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(EventEmitterModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventEmitterModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class EventEmitterModelExtensions
    {

        public static bool IsEventEmitterModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == EventEmitterModel.SpecializationTypeId;
        }

        public static EventEmitterModel AsEventEmitterModel(this ICanBeReferencedType type)
        {
            return type.IsEventEmitterModel() ? new EventEmitterModel((IElement)type) : null;
        }
    }
}