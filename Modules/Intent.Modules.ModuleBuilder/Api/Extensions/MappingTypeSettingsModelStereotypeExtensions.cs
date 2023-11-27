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
    public static class MappingTypeSettingsModelStereotypeExtensions
    {
        public static MappingTypeSettings GetMappingTypeSettings(this MappingTypeSettingsModel model)
        {
            var stereotype = model.GetStereotype("Mapping Type Settings");
            return stereotype != null ? new MappingTypeSettings(stereotype) : null;
        }


        public static bool HasMappingTypeSettings(this MappingTypeSettingsModel model)
        {
            return model.HasStereotype("Mapping Type Settings");
        }

        public static bool TryGetMappingTypeSettings(this MappingTypeSettingsModel model, out MappingTypeSettings stereotype)
        {
            if (!HasMappingTypeSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MappingTypeSettings(model.GetStereotype("Mapping Type Settings"));
            return true;
        }

        public class MappingTypeSettings
        {
            private IStereotype _stereotype;

            public MappingTypeSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement[] Sources()
            {
                return _stereotype.GetProperty<IElement[]>("Sources") ?? new IElement[0];
            }

            public IElement[] Targets()
            {
                return _stereotype.GetProperty<IElement[]>("Targets") ?? new IElement[0];
            }

            public RepresentsOptions Represents()
            {
                return new RepresentsOptions(_stereotype.GetProperty<string>("Represents"));
            }

            public string LineColor()
            {
                return _stereotype.GetProperty<string>("Line Color");
            }

            public string LineDashArray()
            {
                return _stereotype.GetProperty<string>("Line Dash Array");
            }

            public string ValidationFunction()
            {
                return _stereotype.GetProperty<string>("Validation Function");
            }

            public class RepresentsOptions
            {
                public readonly string Value;

                public RepresentsOptions(string value)
                {
                    Value = value;
                }

                public RepresentsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Data":
                            return RepresentsOptionsEnum.Data;
                        case "Invokable":
                            return RepresentsOptionsEnum.Invokable;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsData()
                {
                    return Value == "Data";
                }
                public bool IsInvokable()
                {
                    return Value == "Invokable";
                }
            }

            public enum RepresentsOptionsEnum
            {
                Data,
                Invokable
            }
        }

    }
}