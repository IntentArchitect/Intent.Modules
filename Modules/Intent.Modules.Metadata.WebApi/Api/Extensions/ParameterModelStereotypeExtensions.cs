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
    public static class ParameterModelStereotypeExtensions
    {
        public static ParameterSettings GetParameterSettings(this ParameterModel model)
        {
            var stereotype = model.GetStereotype("d01df110-1208-4af8-a913-92a49d219552");
            return stereotype != null ? new ParameterSettings(stereotype) : null;
        }

        public static bool HasParameterSettings(this ParameterModel model)
        {
            return model.HasStereotype("d01df110-1208-4af8-a913-92a49d219552");
        }

        public static bool TryGetParameterSettings(this ParameterModel model, out ParameterSettings stereotype)
        {
            if (!HasParameterSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ParameterSettings(model.GetStereotype("d01df110-1208-4af8-a913-92a49d219552"));
            return true;
        }


        public class ParameterSettings
        {
            private IStereotype _stereotype;

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
                public bool IsFromQuery()
                {
                    return Value == "From Query";
                }
                public bool IsFromBody()
                {
                    return Value == "From Body";
                }
                public bool IsFromForm()
                {
                    return Value == "From Form";
                }
                public bool IsFromRoute()
                {
                    return Value == "From Route";
                }
                public bool IsFromHeader()
                {
                    return Value == "From Header";
                }
            }

            public enum SourceOptionsEnum
            {
                Default,
                FromQuery,
                FromBody,
                FromForm,
                FromRoute,
                FromHeader
            }
        }

    }
}