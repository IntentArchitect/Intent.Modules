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
    public static class AssociationVisualSettingsModelStereotypeExtensions
    {
        public static Setting GetSetting(this AssociationVisualSettingsModel model)
        {
            var stereotype = model.GetStereotype(Setting.DefinitionId);
            return stereotype != null ? new Setting(stereotype) : null;
        }

        public static bool HasSetting(this AssociationVisualSettingsModel model)
        {
            return model.HasStereotype(Setting.DefinitionId);
        }

        public static bool TryGetSetting(this AssociationVisualSettingsModel model, out Setting stereotype)
        {
            if (!HasSetting(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Setting(model.GetStereotype(Setting.DefinitionId));
            return true;
        }


        public class Setting
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "e21d9a65-36b8-4664-9c61-d70f3615023c";

            public Setting(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public LineTypeOptions LineType()
            {
                return new LineTypeOptions(_stereotype.GetProperty<string>("Line Type"));
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

            public bool ReverseFlowDirection()
            {
                return _stereotype.GetProperty<bool>("Reverse Flow Direction");
            }

            public class LineTypeOptions
            {
                public readonly string Value;

                public LineTypeOptions(string value)
                {
                    Value = value;
                }

                public LineTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Elbow Connector":
                            return LineTypeOptionsEnum.ElbowConnector;
                        case "Curved":
                            return LineTypeOptionsEnum.Curved;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsElbowConnector()
                {
                    return Value == "Elbow Connector";
                }
                public bool IsCurved()
                {
                    return Value == "Curved";
                }
            }

            public enum LineTypeOptionsEnum
            {
                ElbowConnector,
                Curved
            }

        }

    }
}