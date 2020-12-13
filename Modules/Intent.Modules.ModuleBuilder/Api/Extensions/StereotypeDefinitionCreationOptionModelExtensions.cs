using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class StereotypeDefinitionCreationOptionModelExtensions
    {
        public static OptionSettings GetOptionSettings(this StereotypeDefinitionCreationOptionModel model)
        {
            var stereotype = model.GetStereotype("Option Settings");
            return stereotype != null ? new OptionSettings(stereotype) : null;
        }

        public static bool HasOptionSettings(this StereotypeDefinitionCreationOptionModel model)
        {
            return model.HasStereotype("Option Settings");
        }


        public class OptionSettings
        {
            private IStereotype _stereotype;

            public OptionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

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

            public string ApiModelName()
            {
                return _stereotype.GetProperty<string>("Api Model Name");
            }

        }

    }
}