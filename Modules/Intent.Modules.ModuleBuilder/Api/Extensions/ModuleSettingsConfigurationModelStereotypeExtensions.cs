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
    public static class ModuleSettingsConfigurationModelStereotypeExtensions
    {
        public static Configuration GetConfiguration(this ModuleSettingsConfigurationModel model)
        {
            var stereotype = model.GetStereotype(Configuration.DefinitionId);
            return stereotype != null ? new Configuration(stereotype) : null;
        }


        public static bool HasConfiguration(this ModuleSettingsConfigurationModel model)
        {
            return model.HasStereotype(Configuration.DefinitionId);
        }

        public static bool TryGetConfiguration(this ModuleSettingsConfigurationModel model, out Configuration stereotype)
        {
            if (!HasConfiguration(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Configuration(model.GetStereotype(Configuration.DefinitionId));
            return true;
        }

        public class Configuration
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "15b3e709-aeb8-410a-9172-1273eb7b0864";

            public Configuration(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SettingsTypeOptions SettingsType()
            {
                return new SettingsTypeOptions(_stereotype.GetProperty<string>("Settings Type"));
            }

            public class SettingsTypeOptions
            {
                public readonly string Value;

                public SettingsTypeOptions(string value)
                {
                    Value = value;
                }

                public SettingsTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Application Settings":
                            return SettingsTypeOptionsEnum.ApplicationSettings;
                        case "User Settings":
                            return SettingsTypeOptionsEnum.UserSettings;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsApplicationSettings()
                {
                    return Value == "Application Settings";
                }
                public bool IsUserSettings()
                {
                    return Value == "User Settings";
                }
            }

            public enum SettingsTypeOptionsEnum
            {
                ApplicationSettings,
                UserSettings
            }
        }

    }
}