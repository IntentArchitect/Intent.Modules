using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.ApiGateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class APIResourceModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "API Resource";
        public const string SpecializationTypeId = "58fa3b8b-0ce2-40c1-9113-dcccfc8cae67";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public APIResourceModel(IElement element, string requiredType = SpecializationType)
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

        public IList<APIMethodModel> Methods => _element.ChildElements
            .GetElementsOfType(APIMethodModel.SpecializationTypeId)
            .Select(x => new APIMethodModel(x))
            .ToList();

        public IList<APIResourceModel> Resources => _element.ChildElements
            .GetElementsOfType(APIResourceModel.SpecializationTypeId)
            .Select(x => new APIResourceModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(APIResourceModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((APIResourceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class APIResourceModelExtensions
    {

        public static bool IsAPIResourceModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == APIResourceModel.SpecializationTypeId;
        }

        public static APIResourceModel AsAPIResourceModel(this ICanBeReferencedType type)
        {
            return type.IsAPIResourceModel() ? new APIResourceModel((IElement)type) : null;
        }
    }
}