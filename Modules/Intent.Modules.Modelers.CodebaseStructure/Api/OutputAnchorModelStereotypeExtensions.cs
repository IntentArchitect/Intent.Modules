using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.CodebaseStructure.Api
{
    public static class OutputAnchorModelStereotypeExtensions
    {
        public static OutputAnchorSettings GetOutputAnchorSettings(this OutputAnchorModel model)
        {
            var stereotype = model.GetStereotype(OutputAnchorSettings.DefinitionId);
            return stereotype != null ? new OutputAnchorSettings(stereotype) : null;
        }


        public static bool HasOutputAnchorSettings(this OutputAnchorModel model)
        {
            return model.HasStereotype(OutputAnchorSettings.DefinitionId);
        }

        public static bool TryGetOutputAnchorSettings(this OutputAnchorModel model, out OutputAnchorSettings stereotype)
        {
            if (!HasOutputAnchorSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OutputAnchorSettings(model.GetStereotype(OutputAnchorSettings.DefinitionId));
            return true;
        }

        public class OutputAnchorSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a23c7901-31a5-4cbf-b8bf-1be128977e6d";

            public OutputAnchorSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool CreateSubFolders()
            {
                return _stereotype.GetProperty<bool>("Create Sub-Folders");
            }

        }

    }
}