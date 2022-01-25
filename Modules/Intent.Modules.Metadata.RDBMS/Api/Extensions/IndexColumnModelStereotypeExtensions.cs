using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    public static class IndexColumnModelStereotypeExtensions
    {
        public static IndexColumn GetIndexColumn(this IndexColumnModel model)
        {
            var stereotype = model.GetStereotype("Index Column");
            return stereotype != null ? new IndexColumn(stereotype) : null;
        }

        public static bool HasIndexColumn(this IndexColumnModel model)
        {
            return model.HasStereotype("Index Column");
        }


        public class IndexColumn
        {
            private IStereotype _stereotype;

            public IndexColumn(IStereotype stereotype)
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
                        case "Key":
                            return TypeOptionsEnum.Key;
                        case "Included":
                            return TypeOptionsEnum.Included;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsKey()
                {
                    return Value == "Key";
                }
                public bool IsIncluded()
                {
                    return Value == "Included";
                }
            }

            public enum TypeOptionsEnum
            {
                Key,
                Included
            }
        }

    }
}