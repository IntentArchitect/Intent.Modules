using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.Modules.ModuleBuilder.Api.Factories;
using Intent.RoslynWeaver.Attributes;
using IconType = Intent.IArchitect.Common.Types.IconType;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationSettingsModel : IHasStereotypes, IMetadataModel, ICreatableType
    {
        public const string SpecializationType = "Association Settings";
        protected readonly IElement _element;

        public AssociationSettingsModel(IElement element)
        {
            if (!SpecializationType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a 'AssociationSettingsModel' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string ApiClassName => $"{Name}Model";

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public DesignerModel Designer => DesignerModelFactory.GetDesigner(_element);

        [IntentManaged(Mode.Fully)]
        public AssociationSourceEndSettingsModel SourceEnd => _element.ChildElements
            .Where(x => x.SpecializationType == AssociationSourceEndSettingsModel.SpecializationType)
            .Select(x => new AssociationSourceEndSettingsModel(x))
            .SingleOrDefault();
        [IntentManaged(Mode.Fully)]
        public AssociationDestinationEndSettingsModel DestinationEnd => _element.ChildElements
            .Where(x => x.SpecializationType == AssociationDestinationEndSettingsModel.SpecializationType)
            .Select(x => new AssociationDestinationEndSettingsModel(x))
            .SingleOrDefault();

        public AssociationSettingsPersistable ToPersistable()
        {
            return new AssociationSettingsPersistable
            {
                SpecializationType = this.Name,
                Icon = this.SourceEnd.GetSettings().Icon().ToPersistable(),
                SourceEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = this.SourceEnd.GetSettings().TargetTypes().Select(t => t.Name).ToArray(),
                    IsCollectionDefault = this.SourceEnd.GetSettings().IsCollectionDefault(),
                    IsCollectionEnabled = this.SourceEnd.GetSettings().IsCollectionEnabled(),
                    IsNavigableDefault = this.SourceEnd.GetSettings().IsNavigableEnabled(),
                    IsNavigableEnabled = this.SourceEnd.GetSettings().IsNavigableEnabled(),
                    IsNullableDefault = this.SourceEnd.GetSettings().IsNullableDefault(),
                    IsNullableEnabled = this.SourceEnd.GetSettings().IsNullableEnabled()
                },
                TargetEnd = new AssociationEndSettingsPersistable
                {
                    TargetTypes = this.DestinationEnd.GetSettings().TargetTypes().Select(t => t.Name).ToArray(),
                    IsCollectionDefault = this.DestinationEnd.GetSettings().IsCollectionDefault(),
                    IsCollectionEnabled = this.DestinationEnd.GetSettings().IsCollectionEnabled(),
                    IsNavigableDefault = this.DestinationEnd.GetSettings().IsNavigableEnabled(),
                    IsNavigableEnabled = this.DestinationEnd.GetSettings().IsNavigableEnabled(),
                    IsNullableDefault = this.DestinationEnd.GetSettings().IsNullableDefault(),
                    IsNullableEnabled = this.DestinationEnd.GetSettings().IsNullableEnabled()
                },
            };
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationSettingsModel other)
        {
            return Equals(_element, other._element);
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
    }
}