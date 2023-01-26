using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.StepFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class IsNotPresentConditionModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "IsNotPresent Condition";
        public const string SpecializationTypeId = "35f8005e-f858-407f-a9c5-cf0f6ca57f49";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IsNotPresentConditionModel(IElement element, string requiredType = SpecializationType)
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

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(IsNotPresentConditionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IsNotPresentConditionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IsNotPresentConditionModelExtensions
    {

        public static bool IsIsNotPresentConditionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == IsNotPresentConditionModel.SpecializationTypeId;
        }

        public static IsNotPresentConditionModel AsIsNotPresentConditionModel(this ICanBeReferencedType type)
        {
            return type.IsIsNotPresentConditionModel() ? new IsNotPresentConditionModel((IElement)type) : null;
        }
    }
}