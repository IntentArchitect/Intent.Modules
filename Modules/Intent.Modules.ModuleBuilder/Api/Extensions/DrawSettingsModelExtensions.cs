using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class DrawSettingsModelExtensions
    {
        public static DrawSettings GetDrawSettings(this DrawSettingsModel model)
        {
            var stereotype = model.GetStereotype("Draw Settings");
            return stereotype != null ? new DrawSettings(stereotype) : null;
        }

        public static bool HasDrawSettings(this DrawSettingsModel model)
        {
            return model.HasStereotype("Draw Settings");
        }


        public class DrawSettings
        {
            private IStereotype _stereotype;

            public DrawSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

        }

    }
}