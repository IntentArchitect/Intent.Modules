using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.OutputTargets.Folders.Api
{
    [IntentManaged(Mode.Merge)]
    public class RoleModel : IHasStereotypes, IMetadataModel, IOutputTargetRole, IHasName, IElementWrapper, IHasFolder
    {
        public const string SpecializationType = "Role";
        public const string SpecializationTypeId = "8663c9f9-2852-45e7-aaa1-0883a2e6f1da";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public RoleModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        [IntentManaged(Mode.Ignore)]
        string IOutputTargetRole.Id => _element.Name;

        [IntentManaged(Mode.Ignore)]
        public IEnumerable<string> RequiredFrameworks => new string[0];

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(RoleModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RoleModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;

        public FolderModel Folder { get; }
    }

    [IntentManaged(Mode.Fully)]
    public static class RoleModelExtensions
    {

        public static bool IsRoleModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == RoleModel.SpecializationTypeId;
        }

        public static RoleModel AsRoleModel(this ICanBeReferencedType type)
        {
            return type.IsRoleModel() ? new RoleModel((IElement)type) : null;
        }
    }
}