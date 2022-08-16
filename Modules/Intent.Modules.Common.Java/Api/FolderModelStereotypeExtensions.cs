using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Common.Java.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static FolderOptions GetFolderOptions(this FolderModel model)
        {
            var stereotype = model.GetStereotype("Folder Options");
            return stereotype != null ? new FolderOptions(stereotype) : null;
        }


        public static bool HasFolderOptions(this FolderModel model)
        {
            return model.HasStereotype("Folder Options");
        }

        public static bool TryGetFolderOptions(this FolderModel model, out FolderOptions stereotype)
        {
            if (!HasFolderOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FolderOptions(model.GetStereotype("Folder Options"));
            return true;
        }

        public class FolderOptions
        {
            private IStereotype _stereotype;

            public FolderOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool PackageProvider()
            {
                return _stereotype.GetProperty<bool>("Package Provider");
            }

        }

    }
}