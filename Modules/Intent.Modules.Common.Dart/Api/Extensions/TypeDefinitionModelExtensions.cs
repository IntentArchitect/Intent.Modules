using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Common.Dart.Api.Extensions
{
    public static class TypeDefinitionModelExtensions
    {
        public static Dart GetDart(this TypeDefinitionModel model)
        {
            var stereotype = model.GetStereotype("Dart");
            return stereotype != null ? new Dart(stereotype) : null;
        }

        public static bool HasDart(this TypeDefinitionModel model)
        {
            return model.HasStereotype("Dart");
        }


        public class Dart
        {
            private IStereotype _stereotype;

            public Dart(IStereotype stereotype)
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

