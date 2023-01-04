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
    public class IAMPolicyStatementModel : IMetadataModel, IHasStereotypes, IHasName, IHasFolder
    {
        public const string SpecializationType = "IAM Policy Statement";
        public const string SpecializationTypeId = "b38ebd65-3594-40d9-a688-314cd791ecdf";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IAMPolicyStatementModel(IElement element, string requiredType = SpecializationType)
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

        public IList<ActionModel> Actions => _element.ChildElements
            .GetElementsOfType(ActionModel.SpecializationTypeId)
            .Select(x => new ActionModel(x))
            .ToList();

        public IList<ResourceModel> Resources => _element.ChildElements
            .GetElementsOfType(ResourceModel.SpecializationTypeId)
            .Select(x => new ResourceModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(IAMPolicyStatementModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IAMPolicyStatementModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IAMPolicyStatementModelExtensions
    {

        public static bool IsIAMPolicyStatementModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == IAMPolicyStatementModel.SpecializationTypeId;
        }

        public static IAMPolicyStatementModel AsIAMPolicyStatementModel(this ICanBeReferencedType type)
        {
            return type.IsIAMPolicyStatementModel() ? new IAMPolicyStatementModel((IElement)type) : null;
        }
    }
}