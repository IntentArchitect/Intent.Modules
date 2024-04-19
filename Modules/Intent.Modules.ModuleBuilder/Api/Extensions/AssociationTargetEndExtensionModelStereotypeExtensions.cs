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
    public static class AssociationTargetEndExtensionModelStereotypeExtensions
    {
        public static AssociationEndExtensionSettings GetAssociationEndExtensionSettings(this AssociationTargetEndExtensionModel model)
        {
            var stereotype = model.GetStereotype(AssociationEndExtensionSettings.DefinitionId);
            return stereotype != null ? new AssociationEndExtensionSettings(stereotype) : null;
        }


        public static bool HasAssociationEndExtensionSettings(this AssociationTargetEndExtensionModel model)
        {
            return model.HasStereotype(AssociationEndExtensionSettings.DefinitionId);
        }

        public static bool TryGetAssociationEndExtensionSettings(this AssociationTargetEndExtensionModel model, out AssociationEndExtensionSettings stereotype)
        {
            if (!HasAssociationEndExtensionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AssociationEndExtensionSettings(model.GetStereotype(AssociationEndExtensionSettings.DefinitionId));
            return true;
        }


        public class AssociationEndExtensionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1cd7ee9e-614e-4770-a831-d6cdaa56027c";

            public AssociationEndExtensionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

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

            public AllowNavigableOptions AllowNavigable()
            {
                return new AllowNavigableOptions(_stereotype.GetProperty<string>("Allow Navigable"));
            }

            public AllowNullableOptions AllowNullable()
            {
                return new AllowNullableOptions(_stereotype.GetProperty<string>("Allow Nullable"));
            }

            public AllowCollectionOptions AllowCollection()
            {
                return new AllowCollectionOptions(_stereotype.GetProperty<string>("Allow Collection"));
            }

            public class AllowNavigableOptions
            {
                public readonly string Value;

                public AllowNavigableOptions(string value)
                {
                    Value = value;
                }

                public AllowNavigableOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Inherit":
                            return AllowNavigableOptionsEnum.Inherit;
                        case "Allow":
                            return AllowNavigableOptionsEnum.Allow;
                        case "Disallow":
                            return AllowNavigableOptionsEnum.Disallow;
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

            public enum AllowNavigableOptionsEnum
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
        }

    }
}