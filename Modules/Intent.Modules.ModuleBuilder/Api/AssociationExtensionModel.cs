using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge)]
    public class AssociationExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference
    {
        public const string SpecializationType = "Association Extension";
        public const string SpecializationTypeId = "41cb9c1a-063e-4ff4-889a-7501235351eb";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AssociationExtensionModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;

        public IElement InternalElement => _element;

        public AssociationSourceEndExtensionModel SourceEnd => _element.ChildElements
            .GetElementsOfType(AssociationSourceEndExtensionModel.SpecializationTypeId)
            .Select(x => new AssociationSourceEndExtensionModel(x))
            .SingleOrDefault();

        public AssociationTargetEndExtensionModel TargetEnd => _element.ChildElements
            .GetElementsOfType(AssociationTargetEndExtensionModel.SpecializationTypeId)
            .Select(x => new AssociationTargetEndExtensionModel(x))
            .SingleOrDefault();

        public AssociationEventSettingsModel EventSettings => _element.ChildElements
            .GetElementsOfType(AssociationEventSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationEventSettingsModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AssociationExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Ignore)]
        public AssociationSettingExtensionPersistable ToPersistable()
        {
            return new AssociationSettingExtensionPersistable
            {
                SpecializationTypeId = this.TypeReference.Element.Id,
                SpecializationType = this.TypeReference.Element.Name,
                SourceEndExtension = this.SourceEnd.ToPersistable(),
                TargetEndExtension = this.TargetEnd.ToPersistable(),
                Macros = this.EventSettings?.ToPersistable()
            };
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AssociationExtensionModelExtensions
    {

        public static bool IsAssociationExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AssociationExtensionModel.SpecializationTypeId;
        }

        public static AssociationExtensionModel AsAssociationExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsAssociationExtensionModel() ? new AssociationExtensionModel((IElement)type) : null;
        }
    }
}