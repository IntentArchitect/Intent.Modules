using System;
using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AssociationVisualSettingsModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Association Visual Settings";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public AssociationVisualSettingsModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
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
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(AssociationVisualSettingsModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssociationVisualSettingsModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public AssociationDestinationEndVisualSettingsModel DestinationVisual => _element.ChildElements
            .GetElementsOfType(AssociationDestinationEndVisualSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationDestinationEndVisualSettingsModel(x))
            .SingleOrDefault();

        [IntentManaged(Mode.Fully)]
        public AssociationSourceEndVisualSettingsModel SourceVisual => _element.ChildElements
            .GetElementsOfType(AssociationSourceEndVisualSettingsModel.SpecializationTypeId)
            .Select(x => new AssociationSourceEndVisualSettingsModel(x))
            .SingleOrDefault();

        public AssociationVisualSettingsPersistable ToPersistable()
        {
            return new AssociationVisualSettingsPersistable()
            {
                LineColor = this.GetSetting().LineColor(),
                LineWidth = this.GetSetting().LineWidth(),
                LineDashArray = this.GetSetting().LineDashArray(),
                SourceEnd = SourceVisual != null ? new AssociationEndVisualSettings()
                {
                    PrimaryLabel = SourceVisual.GetLabelSettings().PrimaryLabel(),
                    SecondaryLabel = SourceVisual.GetLabelSettings().SecondaryLabel(),
                    PointIndicator = SourceVisual.GetPointSettings().Path() != null ? new AssociationPointerSettings()
                    {
                        Path = SourceVisual.GetPointSettings().Path(),
                        LineColor = SourceVisual.GetPointSettings().LineColor(),
                        LineWidth = SourceVisual.GetPointSettings().LineWidth(),
                        LineDashArray = SourceVisual.GetPointSettings().LineDashArray(),
                        FillColor = SourceVisual.GetPointSettings().FillColor(),
                    } : null,
                    NavigableIndicator = SourceVisual.GetNavigableIndicatorSettings().Path() != null ? new AssociationPointerSettings()
                    {
                        Path = SourceVisual.GetNavigableIndicatorSettings().Path(),
                        LineColor = SourceVisual.GetNavigableIndicatorSettings().LineColor(),
                        LineWidth = SourceVisual.GetNavigableIndicatorSettings().LineWidth(),
                        LineDashArray = SourceVisual.GetNavigableIndicatorSettings().LineDashArray(),
                        FillColor = SourceVisual.GetNavigableIndicatorSettings().FillColor(),
                    } : null
                } : null,
                TargetEnd = DestinationVisual != null ? new AssociationEndVisualSettings()
                {
                    PrimaryLabel = DestinationVisual.GetLabelSettings().PrimaryLabel(),
                    SecondaryLabel = DestinationVisual.GetLabelSettings().SecondaryLabel(),
                    PointIndicator = DestinationVisual.GetPointSettings().Path() != null ? new AssociationPointerSettings()
                    {
                        Path = DestinationVisual.GetPointSettings().Path(),
                        LineColor = DestinationVisual.GetPointSettings().LineColor(),
                        LineWidth = DestinationVisual.GetPointSettings().LineWidth(),
                        LineDashArray = DestinationVisual.GetPointSettings().LineDashArray(),
                        FillColor = DestinationVisual.GetPointSettings().FillColor(),
                    } : null,
                    NavigableIndicator = DestinationVisual.GetNavigableIndicatorSettings().Path() != null ? new AssociationPointerSettings()
                    {
                        Path = DestinationVisual.GetNavigableIndicatorSettings().Path(),
                        LineColor = DestinationVisual.GetNavigableIndicatorSettings().LineColor(),
                        LineWidth = DestinationVisual.GetNavigableIndicatorSettings().LineWidth(),
                        LineDashArray = DestinationVisual.GetNavigableIndicatorSettings().LineDashArray(),
                        FillColor = DestinationVisual.GetNavigableIndicatorSettings().FillColor(),
                    } : null
                } : null,
            };
        }
        public const string SpecializationTypeId = "153a4063-82a1-4515-87f1-44c189004fad";

        public string Comment => _element.Comment;
    }
}