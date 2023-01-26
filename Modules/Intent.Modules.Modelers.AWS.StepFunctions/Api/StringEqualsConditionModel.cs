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
    public class StringEqualsConditionModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "StringEquals Condition";
        public const string SpecializationTypeId = "3b11a26d-290c-4a13-aedb-516a6d6b173f";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public StringEqualsConditionModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(StringEqualsConditionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringEqualsConditionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StringEqualsConditionModelExtensions
    {

        public static bool IsStringEqualsConditionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == StringEqualsConditionModel.SpecializationTypeId;
        }

        public static StringEqualsConditionModel AsStringEqualsConditionModel(this ICanBeReferencedType type)
        {
            return type.IsStringEqualsConditionModel() ? new StringEqualsConditionModel((IElement)type) : null;
        }
    }
}