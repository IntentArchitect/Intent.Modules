using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class ServicesPackageModelStereotypeExtensions
    {
        public static EndpointSettings GetEndpointSettings(this ServicesPackageModel model)
        {
            var stereotype = model.GetStereotype(EndpointSettings.DefinitionId);
            return stereotype != null ? new EndpointSettings(stereotype) : null;
        }


        public static bool HasEndpointSettings(this ServicesPackageModel model)
        {
            return model.HasStereotype(EndpointSettings.DefinitionId);
        }

        public static bool TryGetEndpointSettings(this ServicesPackageModel model, out EndpointSettings stereotype)
        {
            if (!HasEndpointSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new EndpointSettings(model.GetStereotype(EndpointSettings.DefinitionId));
            return true;
        }

        public class EndpointSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c06e9978-c271-49fc-b5c9-09833b6b8992";

            public EndpointSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ServiceURL()
            {
                return _stereotype.GetProperty<string>("Service URL");
            }

        }

    }
}