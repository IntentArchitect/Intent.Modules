using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class AssociationTargetEndExtensionModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Association Target End Extension";
        public const string SpecializationTypeId = "1cc6c731-af5f-41a0-9e82-44486f45b903";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AssociationTargetEndExtensionModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(AssociationTargetEndExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationTargetEndExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationTargetEndExtensionModelExtensions
    {

        public static bool IsAssociationTargetEndExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AssociationTargetEndExtensionModel.SpecializationTypeId;
        }

        public static AssociationTargetEndExtensionModel AsAssociationTargetEndExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationTargetEndExtensionModel() ? new AssociationTargetEndExtensionModel((IElement)type) : null;
        }
    }
}