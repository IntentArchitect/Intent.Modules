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
    public class ApplicationTemplateSettingsConfigurationModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Application Template Settings Configuration";
        public const string SpecializationTypeId = "69000b7e-8bc8-49d0-80f5-eb65a6f7653a";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public ApplicationTemplateSettingsConfigurationModel(IElement element, string requiredType = SpecializationType)
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

        public string Value => _element.Value;

        public IElement InternalElement => _element;

        public IList<ApplicationTemplateSettingsFieldConfigurationModel> Fields => _element.ChildElements
            .GetElementsOfType(ApplicationTemplateSettingsFieldConfigurationModel.SpecializationTypeId)
            .Select(x => new ApplicationTemplateSettingsFieldConfigurationModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ApplicationTemplateSettingsConfigurationModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApplicationTemplateSettingsConfigurationModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ApplicationTemplateSettingsConfigurationModelExtensions
    {

        public static bool IsApplicationTemplateSettingsConfigurationModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ApplicationTemplateSettingsConfigurationModel.SpecializationTypeId;
        }

        public static ApplicationTemplateSettingsConfigurationModel AsApplicationTemplateSettingsConfigurationModel(this ICanBeReferencedType type)
        {
            return type.IsApplicationTemplateSettingsConfigurationModel() ? new ApplicationTemplateSettingsConfigurationModel((IElement)type) : null;
        }
    }
}