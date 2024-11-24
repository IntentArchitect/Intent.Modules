using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    public static class IndexModelStereotypeExtensions
    {
        public static Settings GetSettings(this IndexModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this IndexModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this IndexModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype(Settings.DefinitionId));
            return true;
        }


        public class Settings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "18a8e9e7-b8db-41ec-976b-2b6ba0cc4e4d";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool UseDefaultName()
            {
                return _stereotype.GetProperty<bool>("Use Default Name");
            }

            public bool Unique()
            {
                return _stereotype.GetProperty<bool>("Unique");
            }

            public FilterOptions Filter()
            {
                return new FilterOptions(_stereotype.GetProperty<string>("Filter"));
            }

            public string FilterCustomValue()
            {
                return _stereotype.GetProperty<string>("Filter Custom Value");
            }

            public int? FillFactor()
            {
                return _stereotype.GetProperty<int?>("Fill Factor");
            }

            public class FilterOptions
            {
                public readonly string Value;

                public FilterOptions(string value)
                {
                    Value = value;
                }

                public FilterOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return FilterOptionsEnum.Default;
                        case "None":
                            return FilterOptionsEnum.None;
                        case "Custom":
                            return FilterOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum FilterOptionsEnum
            {
                Default,
                None,
                Custom
            }
        }

    }
}