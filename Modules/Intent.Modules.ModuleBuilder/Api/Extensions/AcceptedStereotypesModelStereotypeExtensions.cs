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
    public static class AcceptedStereotypesModelStereotypeExtensions
    {
        public static Settings GetSettings(this AcceptedStereotypesModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }


        public static bool HasSettings(this AcceptedStereotypesModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this AcceptedStereotypesModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype(Settings.DefinitionId));
            return true;
        }

        public class Settings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c8e14e3f-f5ee-4992-b6cb-761ea48a11cd";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IStereotypeDefinition TypesWithStereotype()
            {
                return _stereotype.GetProperty<IStereotypeDefinition>("Types with Stereotype");
            }

        }

    }
}