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
        public static ParameterSettings GetParameterSettings(this DTOFieldModel model)
        {
            var stereotype = model.GetStereotype(ParameterSettings.DefinitionId);
            return stereotype != null ? new ParameterSettings(stereotype) : null;
        }


        public static bool HasParameterSettings(this DTOFieldModel model)
        {
            return model.HasStereotype(ParameterSettings.DefinitionId);
        }

        public static bool TryGetParameterSettings(this DTOFieldModel model, out ParameterSettings stereotype)
        {
            if (!HasParameterSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ParameterSettings(model.GetStereotype(ParameterSettings.DefinitionId));
            return true;
        }
        public static SerializationSettings GetSerializationSettings(this DTOFieldModel model)
        {
            var stereotype = model.GetStereotype(SerializationSettings.DefinitionId);
            return stereotype != null ? new SerializationSettings(stereotype) : null;
        }


        public static bool HasSerializationSettings(this DTOFieldModel model)
        {
            return model.HasStereotype(SerializationSettings.DefinitionId);
        }

        public static bool TryGetSerializationSettings(this DTOFieldModel model, out SerializationSettings stereotype)
        {
            if (!HasSerializationSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SerializationSettings(model.GetStereotype(SerializationSettings.DefinitionId));
            return true;
        }

        public class ParameterSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d01df110-1208-4af8-a913-92a49d219552";

            public ParameterSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SourceOptions Source()
            {
                return new SourceOptions(_stereotype.GetProperty<string>("Source"));
            }

            public string HeaderName()
            {
                return _stereotype.GetProperty<string>("Header Name");
            }

            public string QueryStringName()
            {
                return _stereotype.GetProperty<string>("Query String Name");
            }

            public class SourceOptions
            {
                public readonly string Value;

                public SourceOptions(string value)
                {
                    Value = value;
                }

                public SourceOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return SourceOptionsEnum.Default;
                        case "From Body":
                            return SourceOptionsEnum.FromBody;
                        case "From Form":
                            return SourceOptionsEnum.FromForm;
                        case "From Header":
                            return SourceOptionsEnum.FromHeader;
                        case "From Query":
                            return SourceOptionsEnum.FromQuery;
                        case "From Route":
                            return SourceOptionsEnum.FromRoute;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsFromBody()
                {
                    return Value == "From Body";
                }
                public bool IsFromForm()
                {
                    return Value == "From Form";
                }
                public bool IsFromHeader()
                {
                    return Value == "From Header";
                }
                public bool IsFromQuery()
                {
                    return Value == "From Query";
                }
                public bool IsFromRoute()
                {
                    return Value == "From Route";
                }
            }

            public enum SourceOptionsEnum
            {
                Default,
                FromBody,
                FromForm,
                FromHeader,
                FromQuery,
                FromRoute
            }
        }

        public class SerializationSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "e3b79cb0-b063-4aaa-8fda-3893fb6e44c4";

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