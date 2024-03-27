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
    public class SQSSendMessageRetryModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "SQS Send Message Retry";
        public const string SpecializationTypeId = "2ec695a5-cabb-4ed4-896e-0381af3b188e";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public SQSSendMessageRetryModel(IElement element, string requiredType = SpecializationType)
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

        public IList<SQSSendMessageErrorModel> Errors => _element.ChildElements
            .GetElementsOfType(SQSSendMessageErrorModel.SpecializationTypeId)
            .Select(x => new SQSSendMessageErrorModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(SQSSendMessageRetryModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SQSSendMessageRetryModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class SQSSendMessageRetryModelExtensions
    {

        public static bool IsSQSSendMessageRetryModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == SQSSendMessageRetryModel.SpecializationTypeId;
        }

        public static SQSSendMessageRetryModel AsSQSSendMessageRetryModel(this ICanBeReferencedType type)
        {
            return type.IsSQSSendMessageRetryModel() ? new SQSSendMessageRetryModel((IElement)type) : null;
        }
    }
}