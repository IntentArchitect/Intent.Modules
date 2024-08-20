using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    public static class NuGetPackageModelStereotypeExtensions
    {
        public static PackageSettings GetPackageSettings(this NuGetPackageModel model)
        {
            var stereotype = model.GetStereotype(PackageSettings.DefinitionId);
            return stereotype != null ? new PackageSettings(stereotype) : null;
        }


        public static bool HasPackageSettings(this NuGetPackageModel model)
        {
            return model.HasStereotype(PackageSettings.DefinitionId);
        }

        public static bool TryGetPackageSettings(this NuGetPackageModel model, out PackageSettings stereotype)
        {
            if (!HasPackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PackageSettings(model.GetStereotype(PackageSettings.DefinitionId));
            return true;
        }

        public class PackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "265221a5-779c-46c9-a367-8b07b435803b";

            public PackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string FriendlyName()
            {
                return _stereotype.GetProperty<string>("Friendly Name");
            }

            public bool Locked()
            {
                return _stereotype.GetProperty<bool>("Locked");
            }

        }

    }
}