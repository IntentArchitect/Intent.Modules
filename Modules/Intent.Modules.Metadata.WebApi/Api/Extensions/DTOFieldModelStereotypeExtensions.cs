using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class DTOFieldModelStereotypeExtensions
    {
        public static SerializationSettings GetSerializationSettings(this DTOFieldModel model)
        {
            var stereotype = model.GetStereotype("Serialization Settings");
            return stereotype != null ? new SerializationSettings(stereotype) : null;
        }


        public static bool HasSerializationSettings(this DTOFieldModel model)
        {
            return model.HasStereotype("Serialization Settings");
        }

        public static bool TryGetSerializationSettings(this DTOFieldModel model, out SerializationSettings stereotype)
        {
            if (!HasSerializationSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SerializationSettings(model.GetStereotype("Serialization Settings"));
            return true;
        }

        public class SerializationSettings
        {
            private IStereotype _stereotype;

            public SerializationSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public FieldNamingConventionOptions FieldNamingConvention()
            {
                return new FieldNamingConventionOptions(_stereotype.GetProperty<string>("Field Naming Convention"));
            }

            public NamingConventionOptions NamingConvention()
            {
                return new NamingConventionOptions(_stereotype.GetProperty<string>("Naming Convention"));
            }

            public string CustomName()
            {
                return _stereotype.GetProperty<string>("Custom Name");
            }

            public class FieldNamingConventionOptions
            {
                public readonly string Value;

                public FieldNamingConventionOptions(string value)
                {
                    Value = value;
                }

                public FieldNamingConventionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Camel Case":
                            return FieldNamingConventionOptionsEnum.CamelCase;
                        case "Kebab Case":
                            return FieldNamingConventionOptionsEnum.KebabCase;
                        case "Pascal Case":
                            return FieldNamingConventionOptionsEnum.PascalCase;
                        case "Snake Case":
                            return FieldNamingConventionOptionsEnum.SnakeCase;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsCamelCase()
                {
                    return Value == "Camel Case";
                }
                public bool IsKebabCase()
                {
                    return Value == "Kebab Case";
                }
                public bool IsPascalCase()
                {
                    return Value == "Pascal Case";
                }
                public bool IsSnakeCase()
                {
                    return Value == "Snake Case";
                }
            }

            public enum FieldNamingConventionOptionsEnum
            {
                CamelCase,
                KebabCase,
                PascalCase,
                SnakeCase
            }
            public class NamingConventionOptions
            {
                public readonly string Value;

                public NamingConventionOptions(string value)
                {
                    Value = value;
                }

                public NamingConventionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Camel Case":
                            return NamingConventionOptionsEnum.CamelCase;
                        case "Kebab Case":
                            return NamingConventionOptionsEnum.KebabCase;
                        case "Pascal Case":
                            return NamingConventionOptionsEnum.PascalCase;
                        case "Snake Case":
                            return NamingConventionOptionsEnum.SnakeCase;
                        case "Custom":
                            return NamingConventionOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsCamelCase()
                {
                    return Value == "Camel Case";
                }
                public bool IsKebabCase()
                {
                    return Value == "Kebab Case";
                }
                public bool IsPascalCase()
                {
                    return Value == "Pascal Case";
                }
                public bool IsSnakeCase()
                {
                    return Value == "Snake Case";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum NamingConventionOptionsEnum
            {
                CamelCase,
                KebabCase,
                PascalCase,
                SnakeCase,
                Custom
            }
        }

    }
}