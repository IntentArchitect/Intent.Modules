using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSLambdaFunctionModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "AWS Lambda Function";
        public const string SpecializationTypeId = "de02db8b-e18f-423c-b4da-5f717a04075e";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AWSLambdaFunctionModel(IElement element, string requiredType = SpecializationType)
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

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        public RequestBodyModel RequestBody => _element.ChildElements
            .GetElementsOfType(RequestBodyModel.SpecializationTypeId)
            .Select(x => new RequestBodyModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AWSLambdaFunctionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AWSLambdaFunctionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AWSLambdaFunctionModelExtensions
    {

        public static bool IsAWSLambdaFunctionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AWSLambdaFunctionModel.SpecializationTypeId;
        }

        public static AWSLambdaFunctionModel AsAWSLambdaFunctionModel(this ICanBeReferencedType type)
        {
            return type.IsAWSLambdaFunctionModel() ? new AWSLambdaFunctionModel((IElement)type) : null;
        }
    }
}