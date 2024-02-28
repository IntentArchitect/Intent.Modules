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
            var stereotype = model.GetStereotype("822c6b64-82c2-4e5b-9d07-b79adc9a09e7");
            return stereotype != null ? new TypeScript(stereotype) : null;
        }

        public static bool HasTypeScript(this EnumModel model)
        {
            return model.HasStereotype("822c6b64-82c2-4e5b-9d07-b79adc9a09e7");
        }

        public static bool TryGetTypeScript(this EnumModel model, out TypeScript stereotype)
        {
            if (!HasTypeScript(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TypeScript(model.GetStereotype("822c6b64-82c2-4e5b-9d07-b79adc9a09e7"));
            return true;
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