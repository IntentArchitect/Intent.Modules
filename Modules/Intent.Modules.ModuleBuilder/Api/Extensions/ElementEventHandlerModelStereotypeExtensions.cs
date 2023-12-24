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
    public static class ElementEventHandlerModelStereotypeExtensions
    {
        public static ScriptSettings GetScriptSettings(this ElementEventHandlerModel model)
        {
            var stereotype = model.GetStereotype("56bc0465-c1fe-4bca-9493-2a3ce88a4047");
            return stereotype != null ? new ScriptSettings(stereotype) : null;
        }

        public static bool HasScriptSettings(this ElementEventHandlerModel model)
        {
            return model.HasStereotype("56bc0465-c1fe-4bca-9493-2a3ce88a4047");
        }

        public static bool TryGetScriptSettings(this ElementEventHandlerModel model, out ScriptSettings stereotype)
        {
            if (!HasScriptSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ScriptSettings(model.GetStereotype("56bc0465-c1fe-4bca-9493-2a3ce88a4047"));
            return true;
        }


        public class ScriptSettings
        {
            private IStereotype _stereotype;

            public ScriptSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Script()
            {
                return _stereotype.GetProperty<string>("Script");
            }

        }

    }
}