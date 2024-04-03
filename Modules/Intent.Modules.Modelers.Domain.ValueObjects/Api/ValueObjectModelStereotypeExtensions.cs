using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.Domain.ValueObjects.Api
{
    public static class ValueObjectModelStereotypeExtensions
    {
        public static SerializationSettings GetSerializationSettings(this ValueObjectModel model)
        {
            var stereotype = model.GetStereotype("4ced3df6-2827-461d-bf1b-6512f521f2c6");
            return stereotype != null ? new SerializationSettings(stereotype) : null;
        }


        public static bool HasSerializationSettings(this ValueObjectModel model)
        {
            return model.HasStereotype("4ced3df6-2827-461d-bf1b-6512f521f2c6");
        }

        public static bool TryGetSerializationSettings(this ValueObjectModel model, out SerializationSettings stereotype)
        {
            if (!HasSerializationSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SerializationSettings(model.GetStereotype("4ced3df6-2827-461d-bf1b-6512f521f2c6"));
            return true;
        }

        public class SerializationSettings
        {
            private IStereotype _stereotype;

            public SerializationSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "JSON":
                            return TypeOptionsEnum.JSON;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsJSON()
                {
                    return Value == "JSON";
                }
            }

            public enum TypeOptionsEnum
            {
                JSON
            }
        }

    }
}