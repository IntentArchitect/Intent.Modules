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
    public static class FileClassificationModelStereotypeExtensions
    {
        public static FileClassificationSettings GetFileClassificationSettings(this FileClassificationModel model)
        {
            var stereotype = model.GetStereotype(FileClassificationSettings.DefinitionId);
            return stereotype != null ? new FileClassificationSettings(stereotype) : null;
        }


        public static bool HasFileClassificationSettings(this FileClassificationModel model)
        {
            return model.HasStereotype(FileClassificationSettings.DefinitionId);
        }

        public static bool TryGetFileClassificationSettings(this FileClassificationModel model, out FileClassificationSettings stereotype)
        {
            if (!HasFileClassificationSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileClassificationSettings(model.GetStereotype(FileClassificationSettings.DefinitionId));
            return true;
        }

        public class FileClassificationSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1a1b0c10-113c-472f-8f76-9fb4eb6832f3";

            public FileClassificationSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Color()
            {
                return _stereotype.GetProperty<string>("Color");
            }

            public string ColorDarkMode()
            {
                return _stereotype.GetProperty<string>("Color (Dark Mode)");
            }

        }

    }
}