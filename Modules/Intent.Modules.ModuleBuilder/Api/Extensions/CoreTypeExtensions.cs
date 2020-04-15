using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class CoreTypeExtensions
    {
        public static DefaultCreationOptions GetDefaultCreationOptions(this CoreTypeModel model)
        {
            var stereotype = model.GetStereotype("Default Creation Options");
            return stereotype != null ? new DefaultCreationOptions(stereotype) : null;
        }


        public class DefaultCreationOptions
        {
            private IStereotype _stereotype;

            public DefaultCreationOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
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

        }

    }
}