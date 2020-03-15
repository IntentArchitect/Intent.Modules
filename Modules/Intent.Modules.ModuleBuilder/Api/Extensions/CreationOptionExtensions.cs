using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class CreationOptionExtensions
    {
        public static ApiSettings GetApiSettings(this ICreationOption model)
        {
            var stereotype = model.GetStereotype("Api Settings");
            return stereotype != null ? new ApiSettings(stereotype) : null;
        }

        public static CreationOptions GetCreationOptions(this ICreationOption model)
        {
            var stereotype = model.GetStereotype("Creation Options");
            return stereotype != null ? new CreationOptions(stereotype) : null;
        }


        public class ApiSettings
        {
            private IStereotype _stereotype;

            public ApiSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string PropertyName()
            {
                return _stereotype.GetProperty<string>("PropertyName");
            }

        }

        public class CreationOptions
        {
            private IStereotype _stereotype;

            public CreationOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Text()
            {
                return _stereotype.GetProperty<string>("Text");
            }

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public int? TypeOrder()
            {
                return _stereotype.GetProperty<int?>("Type Order");
            }

            public bool AllowMultiple()
            {
                return _stereotype.GetProperty<bool>("Allow Multiple");
            }

        }

    }
}