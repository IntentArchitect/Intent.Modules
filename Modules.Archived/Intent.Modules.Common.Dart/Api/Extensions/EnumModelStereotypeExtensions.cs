using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Common.Dart.Api
{
    public static class EnumModelStereotypeExtensions
    {
        public static Dart GetDart(this EnumModel model)
        {
            var stereotype = model.GetStereotype("Dart");
            return stereotype != null ? new Dart(stereotype) : null;
        }


        public static bool HasDart(this EnumModel model)
        {
            return model.HasStereotype("Dart");
        }

        public static bool TryGetDart(this EnumModel model, out Dart stereotype)
        {
            if (!HasDart(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Dart(model.GetStereotype("Dart"));
            return true;
        }

        public class Dart
        {
            private IStereotype _stereotype;

            public Dart(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}