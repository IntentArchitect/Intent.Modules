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
    public static class IndexColumnModelStereotypeExtensions
    {
        public static Settings GetSettings(this IndexColumnModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this IndexColumnModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this IndexColumnModel model, out Settings stereotype)
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
            public const string DefinitionId = "1c39a537-7016-4774-a874-23248040c07e";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public SortDirectionOptions SortDirection()
            {
                return new SortDirectionOptions(_stereotype.GetProperty<string>("Sort Direction"));
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Key":
                            return TypeOptionsEnum.Key;
                        case "Included":
                            return TypeOptionsEnum.Included;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsKey()
                {
                    return Value == "Key";
                }
                public bool IsIncluded()
                {
                    return Value == "Included";
                }
            }

            public enum TypeOptionsEnum
            {
                Key,
                Included
            }
            public class SortDirectionOptions
            {
                public readonly string Value;

                public SortDirectionOptions(string value)
                {
                    Value = value;
                }

                public SortDirectionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Ascending":
                            return SortDirectionOptionsEnum.Ascending;
                        case "Descending":
                            return SortDirectionOptionsEnum.Descending;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAscending()
                {
                    return Value == "Ascending";
                }
                public bool IsDescending()
                {
                    return Value == "Descending";
                }
            }

            public enum SortDirectionOptionsEnum
            {
                Ascending,
                Descending
            }
        }

    }
}