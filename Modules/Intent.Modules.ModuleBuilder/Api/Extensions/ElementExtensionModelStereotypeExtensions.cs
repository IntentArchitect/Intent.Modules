using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ElementExtensionModelStereotypeExtensions
    {
        public static ExtensionSettings GetExtensionSettings(this ElementExtensionModel model)
        {
            var stereotype = model.GetStereotype(ExtensionSettings.DefinitionId);
            return stereotype != null ? new ExtensionSettings(stereotype) : null;
        }


        public static bool HasExtensionSettings(this ElementExtensionModel model)
        {
            return model.HasStereotype(ExtensionSettings.DefinitionId);
        }

        public static bool TryGetExtensionSettings(this ElementExtensionModel model, out ExtensionSettings stereotype)
        {
            if (!HasExtensionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ExtensionSettings(model.GetStereotype(ExtensionSettings.DefinitionId));
            return true;
        }
        public static TypeReferenceExtensionSettings GetTypeReferenceExtensionSettings(this ElementExtensionModel model)
        {
            var stereotype = model.GetStereotype(TypeReferenceExtensionSettings.DefinitionId);
            return stereotype != null ? new TypeReferenceExtensionSettings(stereotype) : null;
        }

        public static bool HasTypeReferenceExtensionSettings(this ElementExtensionModel model)
        {
            return model.HasStereotype(TypeReferenceExtensionSettings.DefinitionId);
        }

        public static bool TryGetTypeReferenceExtensionSettings(this ElementExtensionModel model, out TypeReferenceExtensionSettings stereotype)
        {
            if (!HasTypeReferenceExtensionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TypeReferenceExtensionSettings(model.GetStereotype(TypeReferenceExtensionSettings.DefinitionId));
            return true;
        }


        public class ExtensionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "312eab00-f6db-492e-bbb4-e41383c8d3d8";

            public ExtensionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string DisplayTextFunction()
            {
                return _stereotype.GetProperty<string>("Display Text Function");
            }

            public string ValidateFunction()
            {
                return _stereotype.GetProperty<string>("Validate Function");
            }

        }


        public class TypeReferenceExtensionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "159cbc8e-a910-40f7-8e45-3edadbb863c2";

            public TypeReferenceExtensionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ModeOptions Mode()
            {
                return new ModeOptions(_stereotype.GetProperty<string>("Mode"));
            }

            public string DisplayName()
            {
                return _stereotype.GetProperty<string>("Display Name");
            }

            public string Hint()
            {
                return _stereotype.GetProperty<string>("Hint");
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types") ?? new IElement[0];
            }

            public IStereotypeDefinition[] TargetTraits()
            {
                return _stereotype.GetProperty<IStereotypeDefinition[]>("Target Traits");
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

                public ModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Optional":
                            return ModeOptionsEnum.Optional;
                        case "Required":
                            return ModeOptionsEnum.Required;
                        case "Inherit":
                            return ModeOptionsEnum.Inherit;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum ModeOptionsEnum
            {
                Optional,
                Required,
                Inherit
            }
            public class AllowCollectionOptions
            {
                public readonly string Value;

                public AllowCollectionOptions(string value)
                {
                    Value = value;
                }

                public AllowCollectionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Inherit":
                            return AllowCollectionOptionsEnum.Inherit;
                        case "Allow":
                            return AllowCollectionOptionsEnum.Allow;
                        case "Disallow":
                            return AllowCollectionOptionsEnum.Disallow;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum AllowCollectionOptionsEnum
            {
                Inherit,
                Allow,
                Disallow
            }
            public class AllowNullableOptions
            {
                public readonly string Value;

                public AllowNullableOptions(string value)
                {
                    Value = value;
                }

                public AllowNullableOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Inherit":
                            return AllowNullableOptionsEnum.Inherit;
                        case "Allow":
                            return AllowNullableOptionsEnum.Allow;
                        case "Disallow":
                            return AllowNullableOptionsEnum.Disallow;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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

            public enum AllowNullableOptionsEnum
            {
                Inherit,
                Allow,
                Disallow
            }
        }

    }
}