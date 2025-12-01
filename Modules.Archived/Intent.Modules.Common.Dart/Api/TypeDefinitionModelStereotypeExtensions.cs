using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Common.Dart.Api
{
    public static class TypeDefinitionModelStereotypeExtensions
    {
        public static Dart GetDart(this TypeDefinitionModel model)
        {
            var stereotype = model.GetStereotype("0c734e67-bc65-4132-ab09-645a61945567");
            return stereotype != null ? new Dart(stereotype) : null;
        }


        public static bool HasDart(this TypeDefinitionModel model)
        {
            return model.HasStereotype("0c734e67-bc65-4132-ab09-645a61945567");
        }

        public static bool TryGetDart(this TypeDefinitionModel model, out Dart stereotype)
        {
            if (!HasDart(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Dart(model.GetStereotype("0c734e67-bc65-4132-ab09-645a61945567"));
            return true;
        }

        public class Dart
        {
            private IStereotype _stereotype;

            public Dart(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string ImportSource()
            {
                return _stereotype.GetProperty<string>("Import Source");
            }

        }

    }
}