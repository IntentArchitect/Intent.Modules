using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.IArchitect.Common.Types;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using IconType = Intent.IArchitect.Common.Types.IconType;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DesignerModel : IHasStereotypes, IMetadataModel
    {
        protected readonly IElement _element;
        public const string SpecializationType = "Designer";

        public DesignerModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{nameof(DesignerModel)}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            _element = element;

            Name = element.Name;
            //PackageSettings = Api.PackageSettings.Create(element.ChildElements.SingleOrDefault(x => x.SpecializationType == Api.PackageSettings.SpecializationType));
            //ElementSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == Api.ElementSettings.RequiredSpecializationType)
            //    .Select(x => new ElementSettings(x)).OrderBy(x => x.Name)
            //    .ToList<IElementSettings>();
            //AssociationSettings = element.ChildElements
            //    .Where(x => x.SpecializationType == AssociationSetting.RequiredSpecializationType)
            //    .Select(x => new AssociationSetting(x)).OrderBy(x => x.SpecializationType)
            //    .ToList();
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name { get; }

        [IntentManaged(Mode.Fully)]
        public PackageSettingsModel PackageSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.PackageSettingsModel.SpecializationType)
            .Select(x => new PackageSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<AssociationSettingsModel> AssociationTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AssociationSettingsModel.SpecializationType)
            .Select(x => new AssociationSettingsModel(x))
            .ToList<AssociationSettingsModel>();

        [IntentManaged(Mode.Fully)]
        public IList<ElementSettingsModel> ElementTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettingsModel.SpecializationType)
            .Select(x => new ElementSettingsModel(x))
            .ToList<ElementSettingsModel>();

        public string ApiNamespace => this.GetModelerSettings().APINamespace();
        public string ModuleDependency => null;
        public string ModuleVersion => null;
        public string NuGetDependency => null;
        public string NuGetVersion => null;

        protected bool Equals(DesignerModel other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DesignerModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<CoreTypeModel> CoreTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.CoreTypeModel.SpecializationType)
            .Select(x => new CoreTypeModel(x))
            .ToList<CoreTypeModel>();

        public virtual bool IsReference()
        {
            return this.GetModelerSettings().ModelerType().IsReference();
        }

        [IntentManaged(Mode.Fully)]
        public IList<ElementExtensionModel> ElementExtensions => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementExtensionModel.SpecializationType)
            .Select(x => new ElementExtensionModel(x))
            .ToList<ElementExtensionModel>();

        [IntentManaged(Mode.Fully)]
        public ITypeReference TypeReference => _element.TypeReference;
    }

    public class TypeOrder
    {
        public TypeOrder(IElement attribute)
        {
            Order = attribute.GetStereotypeProperty("Creation Options", "Type Order", attribute.TypeReference.Element.GetStereotypeProperty<int?>("Default Creation Options", "Type Order", null));
            Type = attribute.TypeReference.Element.Name;
        }
        public int? Order { get; set; }
        public string Type { get; set; }
    }
}
