using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{
    public static class IndexModelStereotypeExtensions
    {
        public static Index GetIndex(this IndexModel model)
        {
            var stereotype = model.GetStereotype("Index");
            return stereotype != null ? new Index(stereotype) : null;
        }

        public static bool HasIndex(this IndexModel model)
        {
            return model.HasStereotype("Index");
        }


        public class Index
        {
            private IStereotype _stereotype;

            public Index(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool Unique()
            {
                return _stereotype.GetProperty<bool>("Unique");
            }

            public FilterOptions Filter()
            {
                return new FilterOptions(_stereotype.GetProperty<string>("Filter"));
            }

            public string FilterCustomValue()
            {
                return _stereotype.GetProperty<string>("Filter Custom Value");
            }

            public class FilterOptions
            {
                public readonly string Value;

                public FilterOptions(string value)
                {
                    Value = value;
                }

                public FilterOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return FilterOptionsEnum.Default;
                        case "None":
                            return FilterOptionsEnum.None;
                        case "Custom":
                            return FilterOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum FilterOptionsEnum
            {
                Default,
                None,
                Custom
            }
        }

    }
}