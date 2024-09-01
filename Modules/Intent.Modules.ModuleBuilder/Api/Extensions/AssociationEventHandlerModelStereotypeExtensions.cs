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
    public static class AssociationEventHandlerModelStereotypeExtensions
    {
        public static ScriptSettings GetScriptSettings(this AssociationEventHandlerModel model)
        {
            var stereotype = model.GetStereotype(ScriptSettings.DefinitionId);
            return stereotype != null ? new ScriptSettings(stereotype) : null;
        }


        public static bool HasScriptSettings(this AssociationEventHandlerModel model)
        {
            return model.HasStereotype(ScriptSettings.DefinitionId);
        }

        public static bool TryGetScriptSettings(this AssociationEventHandlerModel model, out ScriptSettings stereotype)
        {
            if (!HasScriptSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ScriptSettings(model.GetStereotype(ScriptSettings.DefinitionId));
            return true;
        }


        public class ScriptSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1f88f1ac-b96a-4ae7-9047-883c2fa3b3df";

            public ScriptSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Script()
            {
                return _stereotype.GetProperty<string>("Script");
            }

            public IElement[] Dependencies()
            {
                return _stereotype.GetProperty<IElement[]>("Dependencies") ?? new IElement[0];
            }

        }

    }
}