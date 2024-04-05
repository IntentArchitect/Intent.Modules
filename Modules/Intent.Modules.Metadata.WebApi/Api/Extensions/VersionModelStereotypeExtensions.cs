using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class VersionModelStereotypeExtensions
    {
        public static VersionSettings GetVersionSettings(this VersionModel model)
        {
            var stereotype = model.GetStereotype("1cefa0f4-0fce-4a35-9f8d-389ecfd66b56");
            return stereotype != null ? new VersionSettings(stereotype) : null;
        }


        public static bool HasVersionSettings(this VersionModel model)
        {
            return model.HasStereotype("1cefa0f4-0fce-4a35-9f8d-389ecfd66b56");
        }

        public static bool TryGetVersionSettings(this VersionModel model, out VersionSettings stereotype)
        {
            if (!HasVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new VersionSettings(model.GetStereotype("1cefa0f4-0fce-4a35-9f8d-389ecfd66b56"));
            return true;
        }

        public class VersionSettings
        {
            private IStereotype _stereotype;

            public VersionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsDeprecated()
            {
                return _stereotype.GetProperty<bool>("Is Deprecated");
            }

        }

    }
}