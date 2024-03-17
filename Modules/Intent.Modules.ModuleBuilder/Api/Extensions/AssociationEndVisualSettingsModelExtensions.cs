using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class AssociationEndVisualSettingsModelExtensions
    {
        public static LabelSettings GetLabelSettings(this AssociationEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("Label Settings");
            return stereotype != null ? new LabelSettings(stereotype) : null;
        }

        public static bool HasLabelSettings(this AssociationEndVisualSettingsModel model)
        {
            return model.HasStereotype("Label Settings");
        }

        public static NavigableIndicatorSettings GetNavigableIndicatorSettings(this AssociationEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("Navigable Indicator Settings");
            return stereotype != null ? new NavigableIndicatorSettings(stereotype) : null;
        }

        public static bool HasNavigableIndicatorSettings(this AssociationEndVisualSettingsModel model)
        {
            return model.HasStereotype("Navigable Indicator Settings");
        }

        public static PointSettings GetPointSettings(this AssociationEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("Point Settings");
            return stereotype != null ? new PointSettings(stereotype) : null;
        }

        public static bool HasPointSettings(this AssociationEndVisualSettingsModel model)
        {
            return model.HasStereotype("Point Settings");
        }


        public class LabelSettings
        {
            private IStereotype _stereotype;

            public LabelSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string PrimaryLabel()
            {
                return _stereotype.GetProperty<string>("Primary Label");
            }

            public string SecondaryLabel()
            {
                return _stereotype.GetProperty<string>("Secondary Label");
            }

        }

        public class NavigableIndicatorSettings
        {
            private IStereotype _stereotype;

            public NavigableIndicatorSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Path()
            {
                return _stereotype.GetProperty<string>("Path");
            }

            public string LineColor()
            {
                return _stereotype.GetProperty<string>("Line Color");
            }

            public string LineWidth()
            {
                return _stereotype.GetProperty<string>("Line Width");
            }

            public string LineDashArray()
            {
                return _stereotype.GetProperty<string>("Line Dash Array");
            }

            public string FillColor()
            {
                return _stereotype.GetProperty<string>("Fill Color");
            }

        }

        public class PointSettings
        {
            private IStereotype _stereotype;

            public PointSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Path()
            {
                return _stereotype.GetProperty<string>("Path");
            }

            public string LineColor()
            {
                return _stereotype.GetProperty<string>("Line Color");
            }

            public string LineWidth()
            {
                return _stereotype.GetProperty<string>("Line Width");
            }

            public string LineDashArray()
            {
                return _stereotype.GetProperty<string>("Line Dash Array");
            }

            public string FillColor()
            {
                return _stereotype.GetProperty<string>("Fill Color");
            }

        }

    }
}