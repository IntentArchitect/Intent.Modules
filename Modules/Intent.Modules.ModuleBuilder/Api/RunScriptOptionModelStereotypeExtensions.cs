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
            var stereotype = model.GetStereotype("Option Settings");
            return stereotype != null ? new OptionSettings(stereotype) : null;
        }

        public static bool HasOptionSettings(this RunScriptOptionModel model)
        {
            return model.HasStereotype("Option Settings");
        }

        public static ScriptSettings GetScriptSettings(this RunScriptOptionModel model)
        {
            var stereotype = model.GetStereotype("Script Settings");
            return stereotype != null ? new ScriptSettings(stereotype) : null;
        }

        public static bool HasScriptSettings(this RunScriptOptionModel model)
        {
            return model.HasStereotype("Script Settings");
        }


        public class OptionSettings
        {
            private IStereotype _stereotype;

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