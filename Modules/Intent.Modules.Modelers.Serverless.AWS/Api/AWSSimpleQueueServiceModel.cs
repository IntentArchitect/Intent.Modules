using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Modelers.Serverless.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AWSSimpleQueueServiceModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "AWS Simple Queue Service";
        public const string SpecializationTypeId = "f0627dc5-3d62-4504-a031-90a0a13dfa9d";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AWSSimpleQueueServiceModel(IElement element, string requiredType = SpecializationType)
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

        public IList<AWSSimpleQueueServiceQueueModel> Queues => _element.ChildElements
            .GetElementsOfType(AWSSimpleQueueServiceQueueModel.SpecializationTypeId)
            .Select(x => new AWSSimpleQueueServiceQueueModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AWSSimpleQueueServiceModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AWSSimpleQueueServiceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AWSSimpleQueueServiceModelExtensions
    {

        public static bool IsAWSSimpleQueueServiceModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AWSSimpleQueueServiceModel.SpecializationTypeId;
        }

        public static AWSSimpleQueueServiceModel AsAWSSimpleQueueServiceModel(this ICanBeReferencedType type)
        {
            return type.IsAWSSimpleQueueServiceModel() ? new AWSSimpleQueueServiceModel((IElement)type) : null;
        }
    }
}