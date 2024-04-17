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
    public static class ElementCreationOptionModelStereotypeExtensions
    {
        public static OptionSettings GetOptionSettings(this ElementCreationOptionModel model)
        {
            var stereotype = model.GetStereotype(OptionSettings.DefinitionId);
            return stereotype != null ? new OptionSettings(stereotype) : null;
        }

        public static bool HasOptionSettings(this ElementCreationOptionModel model)
        {
            return model.HasStereotype(OptionSettings.DefinitionId);
        }

        public static bool TryGetOptionSettings(this ElementCreationOptionModel model, out OptionSettings stereotype)
        {
            if (!HasOptionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OptionSettings(model.GetStereotype(OptionSettings.DefinitionId));
            return true;
        }


        public class OptionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f4250b35-559d-4c0b-91ee-c3d7aa239814";

            public OptionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Shortcut()
            {
                return _stereotype.GetProperty<string>("Shortcut");
            }

            public string ShortcutMacOS()
            {
                return _stereotype.GetProperty<string>("Shortcut (macOS)");
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

            public string IsOptionVisibleFunction()
            {
                return _stereotype.GetProperty<string>("Is Option Visible Function");
            }

        }

    }
}