using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
        [Obsolete]
    public static class ParameterModelExtensions
    {
        [Obsolete]
        public static ParameterSettings GetParameterSettings(ParameterModel model)
        {
            var stereotype = model.GetStereotype("Parameter Settings");
            return stereotype != null ? new ParameterSettings(stereotype) : null;
        }

        [Obsolete]
        public static bool HasParameterSettings(ParameterModel model)
        {
            return model.HasStereotype("Parameter Settings");
        }


        [Obsolete]
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

            public class SourceOptions
            {
                public readonly string Value;

                public SourceOptions(string value)
                {
                    Value = value;
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
                public bool IsFromRoute()
                {
                    return Value == "From Route";
                }
                public bool IsFromHeader()
                {
                    return Value == "From Header";
                }
            }

        }

    }
}