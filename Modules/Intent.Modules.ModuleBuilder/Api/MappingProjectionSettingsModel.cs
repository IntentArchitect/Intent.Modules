using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MappingProjectionSettingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Mapping Projection Settings";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public MappingProjectionSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'MappingSettingsModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IList<ElementMappingModel> Mappings => _element.ChildElements
            .GetElementsOfType(ElementMappingModel.SpecializationTypeId)
            .Select(x => new ElementMappingModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public bool Equals(MappingProjectionSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MappingProjectionSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public MappingSettingPersistable ToPersistable()
        {
            return new MappingSettingPersistable()
            {
                Id = Id,
                Name = Name,
                Shortcut = this.GetMappingSettings().Shortcut(),
                OpenOnDoubleClick = this.GetMappingSettings().OpenOnDoubleClick(),
                DefaultModeler = this.GetMappingSettings().DefaultDesigner().Id,
                OptionsSource = GetOptionsSourceEnumValue(),
                LookupElementFunction = this.GetMappingSettings().OptionSource().IsLookupElement()
                    ? this.GetMappingSettings().LookupElementFunction() : null,
                AutoSyncTypeReferences = this.GetMappingSettings().AutoSyncTypeReferences(),
                LookupTypes = this.GetMappingSettings().OptionSource().IsElementsOfType()
                    ? this.GetMappingSettings().LookupTypes().Select(x => new TargetTypeOption() { SpecializationType = x.Id, DisplayText = x.Name }).ToList() : null,
                MapFrom = GetMapFromEnumValue(),
                Symbol = this.GetMappingSettings().Symbol(),
                StyleFunction = this.GetMappingSettings().Style(),
                MappedTypes = this.Mappings.Select(x => x.ToPersistable()).ToList(),
                IsOptionVisibleFunction = this.GetMappingSettings().IsOptionVisibleFunction()
            };
        }

        private MappingMapFrom GetMapFromEnumValue()
        {
            if (this.GetMappingSettings().MapFrom().IsRootElement())
            {
                return MappingMapFrom.RootElement;
            }
            if (this.GetMappingSettings().MapFrom().IsChildElements())
            {
                return MappingMapFrom.ChildElements;
            }
            throw new Exception("Mapping 'Map From' value must be specified.");
        }

        private MappingOptionsSource GetOptionsSourceEnumValue()
        {
            if (this.GetMappingSettings().OptionSource().IsElementsOfType())
            {
                return MappingOptionsSource.ElementsOfType;
            }
            if (this.GetMappingSettings().OptionSource().IsLookupElement())
            {
                return MappingOptionsSource.LookupElement;
            }
            throw new Exception("Mapping 'Option Source' value must be specified.");
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;
        public const string SpecializationTypeId = "178c2f55-9ca1-484d-be43-a91bdd5554dc";

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class MappingProjectionSettingsModelExtensions
    {

        public static bool IsMappingProjectionSettingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == MappingProjectionSettingsModel.SpecializationTypeId;
        }

        public static MappingProjectionSettingsModel AsMappingProjectionSettingsModel(this ICanBeReferencedType type)
        {
            return type.IsMappingProjectionSettingsModel() ? new MappingProjectionSettingsModel((IElement)type) : null;
        }
    }
}