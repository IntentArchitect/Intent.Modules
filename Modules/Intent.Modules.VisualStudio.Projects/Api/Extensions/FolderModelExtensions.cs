using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class FolderModelExtensions
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


        public class FolderOptions
        {
            private IStereotype _stereotype;

            public FolderOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool NamespaceProvider()
            {
                return _stereotype.GetProperty<bool>("Namespace Provider");
            }

        }

    }
}