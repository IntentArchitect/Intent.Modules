using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class IAMPolicyStatementReferenceModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "IAM Policy Statement (Reference)";
        public const string SpecializationTypeId = "b7d9b3e8-3dda-4b76-a86d-0f99aea0091a";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IAMPolicyStatementReferenceModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(IAMPolicyStatementReferenceModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IAMPolicyStatementReferenceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IAMPolicyStatementReferenceModelExtensions
    {

        public static bool IsIAMPolicyStatementReferenceModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == IAMPolicyStatementReferenceModel.SpecializationTypeId;
        }

        public static IAMPolicyStatementReferenceModel AsIAMPolicyStatementReferenceModel(this ICanBeReferencedType type)
        {
            return type.IsIAMPolicyStatementReferenceModel() ? new IAMPolicyStatementReferenceModel((IElement)type) : null;
        }
    }
}