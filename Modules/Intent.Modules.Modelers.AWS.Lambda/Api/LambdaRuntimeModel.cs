using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class LambdaRuntimeModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "Lambda Runtime";
        public const string SpecializationTypeId = "a3555d86-f00e-451f-b9b6-2f0c06b2c79f";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public LambdaRuntimeModel(IElement element, string requiredType = SpecializationType)
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

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(LambdaRuntimeModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LambdaRuntimeModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class LambdaRuntimeModelExtensions
    {

        public static bool IsLambdaRuntimeModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == LambdaRuntimeModel.SpecializationTypeId;
        }

        public static LambdaRuntimeModel AsLambdaRuntimeModel(this ICanBeReferencedType type)
        {
            return type.IsLambdaRuntimeModel() ? new LambdaRuntimeModel((IElement)type) : null;
        }
    }
}