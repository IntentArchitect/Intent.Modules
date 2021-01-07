using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api.Factories;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Association Settings";
        public const string SpecializationTypeId = "16d21684-d5ea-4fde-b648-f88d292fa272";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public AssociationSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'AssociationSettingsModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }


        [IntentManaged(Mode.Ignore)]
        public IntentModuleModel ParentModule => new IntentModuleModel(_element.Package);

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;

        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        public string ApiModelName => $"{Name.ToCSharpIdentifier()}Model";

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public DesignerSettingsModel Designer => DesignerModelFactory.GetDesignerSettings(_element);

        [IntentManaged(Mode.Fully)]
        public AssociationSourceEndSettingsModel SourceEnd => _element.ChildElements
            .GetElementsOfType(AssociationSourceEndSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationSourceEndSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public AssociationDestinationEndSettingsModel TargetEnd => _element.ChildElements
            .GetElementsOfType(AssociationDestinationEndSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationDestinationEndSettingsModel(x))
            .SingleOrDefault();

        public AssociationSettingsPersistable ToPersistable()
        {
            return new AssociationSettingsPersistable
            {
                SpecializationTypeId = this.Id,
                SpecializationType = this.Name,
                Icon = this.SourceEnd.GetSettings().Icon().ToPersistable(),
                SourceEnd = new AssociationEndSettingsPersistable
                {
                    SpecializationTypeId = this.SourceEnd.Id,
                    SpecializationType = this.SourceEnd.Name,
                    TypeReferenceSetting = new TypeReferenceSettingPersistable()
                    {
                        TargetTypes = this.SourceEnd.GetSettings().TargetTypes().Select(t => t.Name).ToArray(),
                        IsCollectionDefault = this.SourceEnd.GetSettings().IsCollectionDefault(),
                        AllowIsCollection = this.SourceEnd.GetSettings().IsCollectionEnabled(),
                        IsNavigableDefault = this.SourceEnd.GetSettings().IsNavigableDefault(),
                        AllowIsNavigable = this.SourceEnd.GetSettings().IsNavigableEnabled(),
                        IsNullableDefault = this.SourceEnd.GetSettings().IsNullableDefault(),
                        AllowIsNullable = this.SourceEnd.GetSettings().IsNullableEnabled()
                    }
                },
                TargetEnd = new AssociationEndSettingsPersistable
                {
                    SpecializationTypeId = this.TargetEnd.Id,
                    SpecializationType = this.TargetEnd.Name,
                    TypeReferenceSetting = new TypeReferenceSettingPersistable()
                    {
                        TargetTypes = this.TargetEnd.GetSettings().TargetTypes().Select(t => t.Name).ToArray(),
                        IsCollectionDefault = this.TargetEnd.GetSettings().IsCollectionDefault(),
                        AllowIsCollection = this.TargetEnd.GetSettings().IsCollectionEnabled(),
                        IsNavigableDefault = this.TargetEnd.GetSettings().IsNavigableDefault(),
                        AllowIsNavigable = this.TargetEnd.GetSettings().IsNavigableEnabled(),
                        IsNullableDefault = this.TargetEnd.GetSettings().IsNullableDefault(),
                        AllowIsNullable = this.TargetEnd.GetSettings().IsNullableEnabled()
                    }
                },
                VisualSettings = VisualSettings?.ToPersistable()
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public AssociationVisualSettingsModel VisualSettings => _element.ChildElements
            .GetElementsOfType(AssociationVisualSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationVisualSettingsModel(x))
            .SingleOrDefault();

    }
}