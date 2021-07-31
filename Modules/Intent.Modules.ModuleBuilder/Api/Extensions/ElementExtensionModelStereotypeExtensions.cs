using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ElementExtensionModelStereotypeExtensions
    {
        public static TypeReferenceExtensionSettings GetTypeReferenceExtensionSettings(this ElementExtensionModel model)
        {
            var stereotype = model.GetStereotype("Type Reference Extension Settings");
            return stereotype != null ? new TypeReferenceExtensionSettings(stereotype) : null;
        }

        public static bool HasTypeReferenceExtensionSettings(this ElementExtensionModel model)
        {
            return model.HasStereotype("Type Reference Extension Settings");
        }


        public class TypeReferenceExtensionSettings
        {
            private IStereotype _stereotype;

            public TypeReferenceExtensionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ModeOptions Mode()
            {
                return new ModeOptions(_stereotype.GetProperty<string>("Mode"));
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types");
            }

            public string DefaultTypeId()
            {
                return _stereotype.GetProperty<string>("Default Type Id");
            }

            public AllowCollectionOptions AllowCollection()
            {
                return new AllowCollectionOptions(_stereotype.GetProperty<string>("Allow Collection"));
            }

            public AllowNullableOptions AllowNullable()
            {
                return new AllowNullableOptions(_stereotype.GetProperty<string>("Allow Nullable"));
            }

            public class ModeOptions
            {
                public readonly string Value;

                public ModeOptions(string value)
                {
                    Value = value;
                }

                public bool IsOptional()
                {
                    return Value == "Optional";
                }
                public bool IsRequired()
                {
                    return Value == "Required";
                }
                public bool IsInherit()
                {
                    return Value == "Inherit";
                }
            }

            public class AllowCollectionOptions
            {
                public readonly string Value;

                public AllowCollectionOptions(string value)
                {
                    Value = value;
                }

                public bool IsInherit()
                {
                    return Value == "Inherit";
                }
                public bool IsAllow()
                {
                    return Value == "Allow";
                }
                public bool IsDisallow()
                {
                    return Value == "Disallow";
                }
            }

            public class AllowNullableOptions
            {
                public readonly string Value;

                public AllowNullableOptions(string value)
                {
                    Value = value;
                }

                public bool IsInherit()
                {
                    return Value == "Inherit";
                }
                public bool IsAllow()
                {
                    return Value == "Allow";
                }
                public bool IsDisallow()
                {
                    return Value == "Disallow";
                }
            }

        }

    }
}