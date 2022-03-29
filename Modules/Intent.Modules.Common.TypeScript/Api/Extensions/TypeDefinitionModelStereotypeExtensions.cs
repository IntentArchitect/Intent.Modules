using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Common.TypeScript.Api
{
    public static class TypeDefinitionModelStereotypeExtensions
    {
        public static TypeScript GetTypeScript(this TypeDefinitionModel model)
        {
            var stereotype = model.GetStereotype("TypeScript");
            return stereotype != null ? new TypeScript(stereotype) : null;
        }

        public static IReadOnlyCollection<TypeScript> GetTypeScripts(this TypeDefinitionModel model)
        {
            var stereotypes = model
                .GetStereotypes("TypeScript")
                .Select(stereotype => new TypeScript(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasTypeScript(this TypeDefinitionModel model)
        {
            return model.HasStereotype("TypeScript");
        }


        public class TypeScript
        {
            private IStereotype _stereotype;

            public TypeScript(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Type()
            {
                return _stereotype.GetProperty<string>("Type");
            }

            public string ImportFrom()
            {
                return _stereotype.GetProperty<string>("Import From");
            }

        }

    }
}