using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.ApiGateway.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class APIGatewayModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "API Gateway";
        public const string SpecializationTypeId = "c0028dcd-38fd-42e4-b7d5-1c639d8899bd";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public APIGatewayModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(APIGatewayModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((APIGatewayModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class APIGatewayModelExtensions
    {

        public static bool IsAPIGatewayModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == APIGatewayModel.SpecializationTypeId;
        }

        public static APIGatewayModel AsAPIGatewayModel(this ICanBeReferencedType type)
        {
            return type.IsAPIGatewayModel() ? new APIGatewayModel((IElement)type) : null;
        }
    }
}