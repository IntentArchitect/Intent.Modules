using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    [IntentManaged(Mode.Merge)]
    public class ComponentModuleModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Component Module";
        public const string SpecializationTypeId = "ef75f8f0-520c-4ab8-814f-5e75f4877dd7";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ComponentModuleModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        [IntentManaged(Mode.Ignore)]
        public string Version => this.GetModuleSettings().Version();

        [IntentManaged(Mode.Ignore)]
        public bool IsIncludedByDefault => this.GetModuleSettings().IncludeByDefault();

        [IntentManaged(Mode.Ignore)]
        public bool IsRequired => this.GetModuleSettings().IsRequired();

        [IntentManaged(Mode.Ignore)]
        public bool EnableFactoryExtensions => this.GetModuleSettings().EnableFactoryExtensions();

        [IntentManaged(Mode.Ignore)]
        public bool InstallApplicationSettings => this.GetModuleSettings().InstallApplicationSettings();

        [IntentManaged(Mode.Ignore)]
        public bool InstallDesignerMetadata => this.GetModuleSettings().InstallDesignerMetadata();

        [IntentManaged(Mode.Ignore)]
        public bool InstallDesigners => this.GetModuleSettings().InstallDesigners();

        [IntentManaged(Mode.Ignore)]
        public bool InstallTemplateOutputs => this.GetModuleSettings().InstallTemplateOutputs();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ComponentModuleModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComponentModuleModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class ComponentModuleModelExtensions
    {

        public static bool IsComponentModuleModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ComponentModuleModel.SpecializationTypeId;
        }

        public static ComponentModuleModel AsComponentModuleModel(this ICanBeReferencedType type)
        {
            return type.IsComponentModuleModel() ? new ComponentModuleModel((IElement)type) : null;
        }
    }
}