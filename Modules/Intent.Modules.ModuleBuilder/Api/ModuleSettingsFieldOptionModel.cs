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
    public partial class ModuleSettingsFieldOptionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Module Settings Field Option";
        public const string SpecializationTypeId = "1592709a-89b1-4cb0-9801-ce9d3b94545a";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ModuleSettingsFieldOptionModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(ModuleSettingsFieldOptionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModuleSettingsFieldOptionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public string Value => _element.Value;
    }

    [IntentManaged(Mode.Fully)]
    public static class ModuleSettingsFieldOptionModelExtensions
    {

        public static bool IsModuleSettingsFieldOptionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ModuleSettingsFieldOptionModel.SpecializationTypeId;
        }

        public static ModuleSettingsFieldOptionModel AsModuleSettingsFieldOptionModel(this ICanBeReferencedType type)
        {
            return type.IsModuleSettingsFieldOptionModel() ? new ModuleSettingsFieldOptionModel((IElement)type) : null;
        }
    }
}