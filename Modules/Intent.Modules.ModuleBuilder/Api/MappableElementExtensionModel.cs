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
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MappableElementExtensionModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Mappable Element Extension";
        public const string SpecializationTypeId = "f9449923-9173-4347-a3a2-38d1eb66f6b5";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public MappableElementExtensionModel(IElement element, string requiredType = SpecializationType)
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

        public IList<RunScriptOptionModel> ScriptOptions => _element.ChildElements
            .GetElementsOfType(RunScriptOptionModel.SpecializationTypeId)
            .Select(x => new RunScriptOptionModel(x))
            .ToList();

        public IList<StaticMappableSettingsModel> StaticMappings => _element.ChildElements
            .GetElementsOfType(StaticMappableSettingsModel.SpecializationTypeId)
            .Select(x => new StaticMappableSettingsModel(x))
            .ToList();

        public IList<MappableElementSettingsModel> ElementMappings => _element.ChildElements
            .GetElementsOfType(MappableElementSettingsModel.SpecializationTypeId)
            .Select(x => new MappableElementSettingsModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(MappableElementExtensionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappableElementExtensionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public MappableElementExtensionSettingsPersistable ToPersistable()
        {
            return new MappableElementExtensionSettingsPersistable()
            {
                ValidateFunction = this.GetMappableExtensionSettings().ValidateFunction(),
                TargetSettings = this.GetMappableExtensionSettings().ExtendTargetSettings()
                    .Select(x => new TargetTypePersistable() { Type = x.DisplayText, TypeId = x.Id })
                    .ToList(),
                ChildSettings = ElementMappings.Select(x => x.ToPersistable()).ToList(),
                ScriptOptions = ScriptOptions.Select(x => x.ToPersistable()).ToList(),
                StaticMappableSettings = StaticMappings.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class MappableElementExtensionModelExtensions
    {

        public static bool IsMappableElementExtensionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappableElementExtensionModel.SpecializationTypeId;
        }

        public static MappableElementExtensionModel AsMappableElementExtensionModel(this ICanBeReferencedType type)
        {
            return type.IsMappableElementExtensionModel() ? new MappableElementExtensionModel((IElement)type) : null;
        }
    }
}