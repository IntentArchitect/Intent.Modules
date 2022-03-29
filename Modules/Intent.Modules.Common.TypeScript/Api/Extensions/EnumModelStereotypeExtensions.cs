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
    public static class EnumModelStereotypeExtensions
    {
        public static TypeScript GetTypeScript(this EnumModel model)
        {
            var stereotype = model.GetStereotype("TypeScript");
            return stereotype != null ? new TypeScript(stereotype) : null;
        }

        public static IReadOnlyCollection<TypeScript> GetTypeScripts(this EnumModel model)
        {
            var stereotypes = model
                .GetStereotypes("TypeScript")
                .Select(stereotype => new TypeScript(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasTypeScript(this EnumModel model)
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