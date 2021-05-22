using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class MappingSettingsModelExtensions
    {
        public static MappingSettings GetMappingSettings(this MappingSettingsModel model)
        {
            var stereotype = model.GetStereotype("Mapping Settings");
            return stereotype != null ? new MappingSettings(stereotype) : null;
        }

        public static bool HasMappingSettings(this MappingSettingsModel model)
        {
            return model.HasStereotype("Mapping Settings");
        }


        public class MappingSettings
        {
            private IStereotype _stereotype;

            public MappingSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement DefaultDesigner()
            {
                return _stereotype.GetProperty<IElement>("Default Designer");
            }

            public OptionSourceOptions OptionSource()
            {
                return new OptionSourceOptions(_stereotype.GetProperty<string>("Option Source"));
            }

            public string LookupElementFunction()
            {
                return _stereotype.GetProperty<string>("Lookup Element Function");
            }

            public IElement[] LookupTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Lookup Types");
            }

            public MapFromOptions MapFrom()
            {
                return new MapFromOptions(_stereotype.GetProperty<string>("Map From"));
            }

            public bool AutoSyncTypeReferences()
            {
                return _stereotype.GetProperty<bool>("Auto-sync Type References");
            }

            public class OptionSourceOptions
            {
                public readonly string Value;

                public OptionSourceOptions(string value)
                {
                    Value = value;
                }

                public bool IsElementsOfType()
                {
                    return Value == "Elements of Type";
                }
                public bool IsLookupElement()
                {
                    return Value == "Lookup Element";
                }
            }

            public class MapFromOptions
            {
                public readonly string Value;

                public MapFromOptions(string value)
                {
                    Value = value;
                }

                public bool IsRootElement()
                {
                    return Value == "Root Element";
                }
                public bool IsChildElements()
                {
                    return Value == "Child Elements";
                }
            }

        }

    }
}