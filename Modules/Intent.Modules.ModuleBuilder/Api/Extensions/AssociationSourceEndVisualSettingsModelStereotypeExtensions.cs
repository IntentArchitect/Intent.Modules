using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class AssociationSourceEndVisualSettingsModelStereotypeExtensions
    {
        public static LabelSettings GetLabelSettings(this AssociationSourceEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("6b16c65c-f348-40b1-96b0-6423917b9356");
            return stereotype != null ? new LabelSettings(stereotype) : null;
        }

        public static bool HasLabelSettings(this AssociationSourceEndVisualSettingsModel model)
        {
            return model.HasStereotype("6b16c65c-f348-40b1-96b0-6423917b9356");
        }

        public static bool TryGetLabelSettings(this AssociationSourceEndVisualSettingsModel model, out LabelSettings stereotype)
        {
            if (!HasLabelSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LabelSettings(model.GetStereotype("6b16c65c-f348-40b1-96b0-6423917b9356"));
            return true;
        }

        public static NavigableIndicatorSettings GetNavigableIndicatorSettings(this AssociationSourceEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("dc5496ee-e524-41ef-8b17-0125a25d0349");
            return stereotype != null ? new NavigableIndicatorSettings(stereotype) : null;
        }

        public static bool HasNavigableIndicatorSettings(this AssociationSourceEndVisualSettingsModel model)
        {
            return model.HasStereotype("dc5496ee-e524-41ef-8b17-0125a25d0349");
        }

        public static bool TryGetNavigableIndicatorSettings(this AssociationSourceEndVisualSettingsModel model, out NavigableIndicatorSettings stereotype)
        {
            if (!HasNavigableIndicatorSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NavigableIndicatorSettings(model.GetStereotype("dc5496ee-e524-41ef-8b17-0125a25d0349"));
            return true;
        }

        public static PointSettings GetPointSettings(this AssociationSourceEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype("1434703f-9e9e-4321-af00-1e857a2e6b80");
            return stereotype != null ? new PointSettings(stereotype) : null;
        }

        public static bool HasPointSettings(this AssociationSourceEndVisualSettingsModel model)
        {
            return model.HasStereotype("1434703f-9e9e-4321-af00-1e857a2e6b80");
        }

        public static bool TryGetPointSettings(this AssociationSourceEndVisualSettingsModel model, out PointSettings stereotype)
        {
            if (!HasPointSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PointSettings(model.GetStereotype("1434703f-9e9e-4321-af00-1e857a2e6b80"));
            return true;
        }


        public class LabelSettings
        {
            private IStereotype _stereotype;

            public LabelSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

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

            public string Name => _stereotype.Name;

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

            public string Name => _stereotype.Name;

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