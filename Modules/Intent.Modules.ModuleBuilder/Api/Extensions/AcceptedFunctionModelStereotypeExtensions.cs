using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class AcceptedFunctionModelStereotypeExtensions
    {
        public static Settings GetSettings(this AcceptedFunctionModel model)
        {
            var stereotype = model.GetStereotype("c8e14e3f-f5ee-4992-b6cb-761ea48a11cd");
            return stereotype != null ? new Settings(stereotype) : null;
        }


        public static bool HasSettings(this AcceptedFunctionModel model)
        {
            return model.HasStereotype("c8e14e3f-f5ee-4992-b6cb-761ea48a11cd");
        }

        public static bool TryGetSettings(this AcceptedFunctionModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype("c8e14e3f-f5ee-4992-b6cb-761ea48a11cd"));
            return true;
        }

        public class Settings
        {
            private IStereotype _stereotype;

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string AcceptsFunction()
            {
                return _stereotype.GetProperty<string>("Accepts Function");
            }

        }

    }
}