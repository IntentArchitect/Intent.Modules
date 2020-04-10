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
    public class Modeler : IHasStereotypes, IMetadataModel
    {
        private readonly IElement _element;
        public const string SpecializationType = "Modeler";

        public Modeler(IElement element)
        {
            if (SpecializationType != element.SpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
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
        public PackageSettings PackageSettings => _element.ChildElements
            .Where(x => x.SpecializationType == Api.PackageSettings.SpecializationType)
            .Select(x => new PackageSettings(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public IList<AssociationSettings> AssociationTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.AssociationSettings.SpecializationType)
            .Select(x => new AssociationSettings(x))
            .ToList<AssociationSettings>();

        [IntentManaged(Mode.Fully)]
        public IList<ElementSettings> ElementTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.ElementSettings.SpecializationType)
            .Select(x => new ElementSettings(x))
            .ToList<ElementSettings>();

        public string ApiNamespace => this.GetModelerSettings().APINamespace();
        public string ModuleDependency => null;
        public string ModuleVersion => null;
        public string NuGetDependency => null;
        public string NuGetVersion => null;

        protected bool Equals(Modeler other)
        {
            return Equals(_element, other._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Modeler)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<CoreType> CoreTypes => _element.ChildElements
            .Where(x => x.SpecializationType == Api.CoreType.SpecializationType)
            .Select(x => new CoreType(x))
            .ToList<CoreType>();
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

    public class IconModel
    {
        public static IconModel CreateIfSpecified(IStereotype stereotype)
        {
            if (stereotype == null)
            {
                return null;
            }
            return new IconModel()
            {
                Type = Enum.Parse<IconType>(stereotype.GetProperty<string>("Type")),
                Source = stereotype.GetProperty<string>("Source"),
            };
        }

        public IconType Type { get; set; }
        public string Source { get; set; }
    }

    public class AssociationSetting
    {
        public const string RequiredSpecializationType = "Association Setting";

        public AssociationSetting(IElement element)
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            SpecializationType = element.Name;
            Icon = IconModel.CreateIfSpecified(element.GetStereotype("Icon (Full)"));
            //SourceEnd =
            throw new NotImplementedException();
        }

        public string SpecializationType { get; set; }

        public IconModel Icon { get; set; }

        public AssociationEndSetting SourceEnd { get; set; }

        public AssociationEndSetting TargetEnd { get; set; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}'";
        }
    }

    public class AssociationEndSetting
    {
        public string[] TargetTypes { get; set; }

        public bool? IsNavigableEnabled { get; set; }

        public bool? IsNullableEnabled { get; set; }

        public bool? IsCollectionEnabled { get; set; }

        public bool? IsNavigableDefault { get; set; }

        public bool? IsNullableDefault { get; set; }

        public bool? IsCollectionDefault { get; set; }
    }
}
