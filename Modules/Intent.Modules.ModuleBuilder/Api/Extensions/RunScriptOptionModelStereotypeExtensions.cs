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
    public static class RunScriptOptionModelStereotypeExtensions
    {
        public static OptionSettings GetOptionSettings(this RunScriptOptionModel model)
        {
            var stereotype = model.GetStereotype(OptionSettings.DefinitionId);
            return stereotype != null ? new OptionSettings(stereotype) : null;
        }

        public static bool HasOptionSettings(this RunScriptOptionModel model)
        {
            return model.HasStereotype(OptionSettings.DefinitionId);
        }

        public static bool TryGetOptionSettings(this RunScriptOptionModel model, out OptionSettings stereotype)
        {
            if (!HasOptionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OptionSettings(model.GetStereotype(OptionSettings.DefinitionId));
            return true;
        }

        public static ScriptSettings GetScriptSettings(this RunScriptOptionModel model)
        {
            var stereotype = model.GetStereotype(ScriptSettings.DefinitionId);
            return stereotype != null ? new ScriptSettings(stereotype) : null;
        }

        public static bool HasScriptSettings(this RunScriptOptionModel model)
        {
            return model.HasStereotype(ScriptSettings.DefinitionId);
        }

        public static bool TryGetScriptSettings(this RunScriptOptionModel model, out ScriptSettings stereotype)
        {
            if (!HasScriptSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ScriptSettings(model.GetStereotype(ScriptSettings.DefinitionId));
            return true;
        }


        public class OptionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "cbe77364-d1d0-400b-a8c4-646a4c869612";

            public OptionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string ShortcutMacOS()
            {
                return _stereotype.GetProperty<string>("Shortcut (macOS)");
            }

            public int? TypeOrder()
            {
                return _stereotype.GetProperty<int?>("Type Order");
            }

            public string IsOptionVisibleFunction()
            {
                return _stereotype.GetProperty<string>("Is Option Visible Function");
            }

        }

        public class ScriptSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "56bc0465-c1fe-4bca-9493-2a3ce88a4047";

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