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
    public static class RootFolderModelStereotypeExtensions
    {
        public static RootFolderOptions GetRootFolderOptions(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(RootFolderOptions.DefinitionId);
            return stereotype != null ? new RootFolderOptions(stereotype) : null;
        }


        public static bool HasRootFolderOptions(this RootFolderModel model)
        {
            return model.HasStereotype(RootFolderOptions.DefinitionId);
        }

        public static bool TryGetRootFolderOptions(this RootFolderModel model, out RootFolderOptions stereotype)
        {
            if (!HasRootFolderOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RootFolderOptions(model.GetStereotype(RootFolderOptions.DefinitionId));
            return true;
        }

        public class RootFolderOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d99df524-886f-40f6-a22a-f591a1746295";

            public RootFolderOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string RelativeLocation()
            {
                return _stereotype.GetProperty<string>("Relative Location");
            }

        }

    }
}