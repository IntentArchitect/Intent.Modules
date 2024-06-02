using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    public static class ComponentViewModelStereotypeExtensions
    {
        public static LoaderSettings GetLoaderSettings(this ComponentViewModel model)
        {
            var stereotype = model.GetStereotype(LoaderSettings.DefinitionId);
            return stereotype != null ? new LoaderSettings(stereotype) : null;
        }


        public static bool HasLoaderSettings(this ComponentViewModel model)
        {
            return model.HasStereotype(LoaderSettings.DefinitionId);
        }

        public static bool TryGetLoaderSettings(this ComponentViewModel model, out LoaderSettings stereotype)
        {
            if (!HasLoaderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new LoaderSettings(model.GetStereotype(LoaderSettings.DefinitionId));
            return true;
        }

        public class LoaderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d3ac312a-62f0-4150-8e0d-e38dfc411997";

            public LoaderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string LoadingIndicator()
            {
                return _stereotype.GetProperty<string>("Loading Indicator");
            }

            public string ErrorMessage()
            {
                return _stereotype.GetProperty<string>("Error Message");
            }

        }

    }
}