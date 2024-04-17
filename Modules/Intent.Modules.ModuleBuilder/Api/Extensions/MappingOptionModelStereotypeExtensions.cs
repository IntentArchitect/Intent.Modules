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
    public static class MappingOptionModelStereotypeExtensions
    {
        public static OptionSettings GetOptionSettings(this MappingOptionModel model)
        {
            var stereotype = model.GetStereotype(OptionSettings.DefinitionId);
            return stereotype != null ? new OptionSettings(stereotype) : null;
        }


        public static bool HasOptionSettings(this MappingOptionModel model)
        {
            return model.HasStereotype(OptionSettings.DefinitionId);
        }

        public static bool TryGetOptionSettings(this MappingOptionModel model, out OptionSettings stereotype)
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
            public const string DefinitionId = "6c209f86-87f6-4fc3-802e-e6e553dfbeca";

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

            public int? TypeOrder()
            {
                return _stereotype.GetProperty<int?>("Type Order");
            }

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string IsOptionVisibleFunction()
            {
                return _stereotype.GetProperty<string>("Is Option Visible Function");
            }

        }

    }
}