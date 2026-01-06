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
    public static class AssociationSettingsModelStereotypeExtensions
    {
        public static AISettings GetAISettings(this AssociationSettingsModel model)
        {
            var stereotype = model.GetStereotype(AISettings.DefinitionId);
            return stereotype != null ? new AISettings(stereotype) : null;
        }


        public static bool HasAISettings(this AssociationSettingsModel model)
        {
            return model.HasStereotype(AISettings.DefinitionId);
        }

        public static bool TryGetAISettings(this AssociationSettingsModel model, out AISettings stereotype)
        {
            if (!HasAISettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AISettings(model.GetStereotype(AISettings.DefinitionId));
            return true;
        }

        public class AISettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "14441454-bfe0-4d5d-8dba-d86d74d33a3b";

            public AISettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Rules()
            {
                return _stereotype.GetProperty<string>("Rules");
            }

        }

    }
}