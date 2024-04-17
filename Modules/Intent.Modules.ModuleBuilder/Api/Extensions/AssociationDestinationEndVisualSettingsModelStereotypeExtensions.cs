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
    public static class AssociationDestinationEndVisualSettingsModelStereotypeExtensions
    {
        public static LabelSettings GetLabelSettings(this AssociationDestinationEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype(LabelSettings.DefinitionId);
            return stereotype != null ? new LabelSettings(stereotype) : null;
        }

        public static bool HasLabelSettings(this AssociationDestinationEndVisualSettingsModel model)
        {
            return model.HasStereotype(LabelSettings.DefinitionId);
        }

        public static bool TryGetLabelSettings(this AssociationDestinationEndVisualSettingsModel model, out LabelSettings stereotype)
        {
            if (!HasLabelSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LabelSettings(model.GetStereotype(LabelSettings.DefinitionId));
            return true;
        }

        public static NavigableIndicatorSettings GetNavigableIndicatorSettings(this AssociationDestinationEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype(NavigableIndicatorSettings.DefinitionId);
            return stereotype != null ? new NavigableIndicatorSettings(stereotype) : null;
        }

        public static bool HasNavigableIndicatorSettings(this AssociationDestinationEndVisualSettingsModel model)
        {
            return model.HasStereotype(NavigableIndicatorSettings.DefinitionId);
        }

        public static bool TryGetNavigableIndicatorSettings(this AssociationDestinationEndVisualSettingsModel model, out NavigableIndicatorSettings stereotype)
        {
            if (!HasNavigableIndicatorSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NavigableIndicatorSettings(model.GetStereotype(NavigableIndicatorSettings.DefinitionId));
            return true;
        }

        public static PointSettings GetPointSettings(this AssociationDestinationEndVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype(PointSettings.DefinitionId);
            return stereotype != null ? new PointSettings(stereotype) : null;
        }

        public static bool HasPointSettings(this AssociationDestinationEndVisualSettingsModel model)
        {
            return model.HasStereotype(PointSettings.DefinitionId);
        }

        public static bool TryGetPointSettings(this AssociationDestinationEndVisualSettingsModel model, out PointSettings stereotype)
        {
            if (!HasPointSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PointSettings(model.GetStereotype(PointSettings.DefinitionId));
            return true;
        }


        public class LabelSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6b16c65c-f348-40b1-96b0-6423917b9356";

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
            public const string DefinitionId = "dc5496ee-e524-41ef-8b17-0125a25d0349";

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
            public const string DefinitionId = "1434703f-9e9e-4321-af00-1e857a2e6b80";

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