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
    public class StaticMappableSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Static Mappable Settings";
        public const string SpecializationTypeId = "c776bc38-c0e8-4535-87dd-79cfd33be2ad";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public StaticMappableSettingsModel(IElement element, string requiredType = SpecializationType)
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

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(StaticMappableSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StaticMappableSettingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentIgnore]
        public MappableElementSettingPersistable ToPersistable()
        {
            return new MappableElementSettingPersistable()
            {
                Version = "2.0.0",
                Id = Id,
                Name = Name,
                SpecializationType = Name,
                SpecializationTypeId = Id,
                Represents = this.GetMappableSettings().Represents().Value.ToLowerInvariant(),
                IconOverride = this.GetMappableSettings().IconOverride().ToPersistable(),
                DisplayFunction = this.GetMappableSettings().DisplayFunction(),
                FilterFunction = this.GetMappableSettings().FilterFunction(),
                IsMappableFunction = this.GetMappableSettings().IsMappableFunction(),
                AllowMultipleMappings = this.GetMappableSettings().AllowMultipleMappings(),
                IsRequiredFunction = this.GetMappableSettings().IsRequiredFunction(),
                IsTraversable = !this.GetMappableSettings().TraversableMode().IsNotTraversable(),
                TraversableTypes = this.GetMappableSettings().TraversableTypes().Select(x => x.Id).ToList(),
                OverrideTypeReferenceFunction = this.GetMappableSettings().OverrideTypeReferenceFunction(),
                GetTraversableTypeFunction = this.GetMappableSettings().GetTraversableTypeFunction(),
                UseChildSettingsFrom = this.GetMappableSettings().UseChildMappingsFrom()?.Id,
                ChildSettings = new List<MappableElementSettingPersistable>(),
                CanBeModified = this.GetMappableSettings().CanBeModified(),
                CreateNameFunction = this.GetMappableSettings().CreateNameFunction(),
                SyncWith = this.GetMappableSettings().SyncMappingTo().IsElementValue() ? MappableElementSettingPersistable.SyncWithValue
                    : this.GetMappableSettings().SyncMappingTo().IsStereotypePropertyValue() ? MappableElementSettingPersistable.SyncWithStereotypeProperty
                    : null,
                SyncStereotypeId = this.GetMappableSettings().SyncStereotype()?.Id,
                SyncStereotypePropertyId = this.GetMappableSettings().SyncStereotypeProperty(),
                OnMappingsChangedScript = this.GetMappableSettings().OnMappingChangedScript(),
                ValidateFunction = this.GetMappableSettings().ValidateFunction(),
                ScriptOptions = ScriptOptions.Select(x => x.ToPersistable()).ToList(),
                StaticMappableSettings = StaticMappings.Select(x => x.ToPersistable()).ToList(),
            };
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class StaticMappableSettingsModelExtensions
    {

        public static bool IsStaticMappableSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == StaticMappableSettingsModel.SpecializationTypeId;
        }

        public static StaticMappableSettingsModel AsStaticMappableSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsStaticMappableSettingsModel() ? new StaticMappableSettingsModel((IElement)type) : null;
        }
    }
}