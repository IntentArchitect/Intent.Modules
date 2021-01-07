using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.IArchitect.Common.Types;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using IconType = Intent.IArchitect.Common.Types.IconType;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DesignerSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Designer Settings";

        [IntentManaged(Mode.Ignore)]
        public DesignerSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{nameof(DesignerSettingsModel)}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;

            //ElementSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.ElementSettings.RequiredSpecializationType)
            //    .Select(x => new ElementSettings(x)).OrderBy(x => x.Name)
            //    .ToList<IElementSettings>();
            //AssociationSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == AssociationSetting.RequiredSpecializationType)
            //    .Select(x => new AssociationSetting(x)).OrderBy(x => x.SpecializationType)
            //    .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        public virtual string MetadataName()
        {
            return Name;
        }

        [IntentManaged(Mode.Fully)]
        public IList<AssociationSettingsModel> AssociationTypes => _element.ChildElements
            .GetElementsOfType(AssociationSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ElementSettingsModel> ElementTypes => _element.ChildElements
            .GetElementsOfType(ElementSettingsModel.SpecializationTypeId)
            .Select(x => new ElementSettingsModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(DesignerSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DesignerSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<CoreTypeModel> CoreTypes => _element.ChildElements
            .GetElementsOfType(CoreTypeModel.SpecializationTypeId)
            .Select(x => new CoreTypeModel(x))
            .ToList();

        public virtual bool IsReference()
        {
            return this.GetDesignerSettings().IsReference();
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public IList<ScriptModel> ScriptTypes => _element.ChildElements
            .GetElementsOfType(ScriptModel.SpecializationTypeId)
            .Select(x => new ScriptModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<ElementExtensionModel> ElementExtensions => _element.ChildElements
            .GetElementsOfType(ElementExtensionModel.SpecializationTypeId)
            .Select(x => new ElementExtensionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<PackageExtensionModel> PackageExtensions => _element.ChildElements
            .GetElementsOfType(PackageExtensionModel.SpecializationTypeId)
            .Select(x => new PackageExtensionModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<PackageSettingsModel> PackageTypes => _element.ChildElements
            .GetElementsOfType(PackageSettingsModel.SpecializationTypeId)
            .Select(x => new PackageSettingsModel(x))
            .ToList();
        public const string SpecializationTypeId = "7a6411a8-ffef-4209-91c6-8d12755a806a";

        public IList<DesignerReferenceModel> DesignerReferences => _element.ChildElements
                    .Where(x => x.SpecializationType == DesignerReferenceModel.SpecializationType)
                    .Select(x => new DesignerReferenceModel(x))
                    .ToList();
    }

    public class TypeOrder : IEquatable<TypeOrder>
    {
        public TypeOrder(ElementCreationOptionModel element)
        {
            Order = element.GetOptionSettings().TypeOrder();
            Type = element.TypeReference.Element.Name;
        }

        public TypeOrder(StereotypeDefinitionCreationOptionModel element)
        {
            Order = element.GetOptionSettings().TypeOrder();
            Type = element.TypeReference.Element.Name;
        }

        public TypeOrder(AssociationCreationOptionModel element)
        {
            Order = element.GetOptionSettings().TypeOrder();
            Type = element.TypeReference.Element.Name;
        }

        public int? Order { get; set; }
        public string Type { get; set; }

        public bool Equals(TypeOrder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeOrder)obj);
        }

        public override int GetHashCode()
        {
            return (Type != null ? Type.GetHashCode() : 0);
        }
    }
}
