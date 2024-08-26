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
    public static class AssociationSourceEndSettingsModelStereotypeExtensions
    {
        public static Settings GetSettings(this AssociationSourceEndSettingsModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this AssociationSourceEndSettingsModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this AssociationSourceEndSettingsModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype(Settings.DefinitionId));
            return true;
        }


        public class Settings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5a410b57-e6a8-4567-b387-71a89290b2c9";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public IElement[] TargetTypes()
            {
                return _stereotype.GetProperty<IElement[]>("Target Types") ?? new IElement[0];
            }

            public IStereotypeDefinition[] TargetTraits()
            {
                return _stereotype.GetProperty<IStereotypeDefinition[]>("Target Traits");
            }

            public string DisplayTextFunction()
            {
                return _stereotype.GetProperty<string>("Display Text Function");
            }

            public string ValidateFunction()
            {
                return _stereotype.GetProperty<string>("Validate Function");
            }

            public string DefaultNameFunction()
            {
                return _stereotype.GetProperty<string>("Default Name Function");
            }

            public bool NameMustBeUnique()
            {
                return _stereotype.GetProperty<bool>("Name Must Be Unique");
            }

            public NameAccessibilityOptions NameAccessibility()
            {
                return new NameAccessibilityOptions(_stereotype.GetProperty<string>("Name Accessibility"));
            }

            public string ApiPropertyName()
            {
                return _stereotype.GetProperty<string>("Api Property Name");
            }

            public bool IsNavigableEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Navigable Enabled");
            }

            public bool IsNullableEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Nullable Enabled");
            }

            public bool IsCollectionEnabled()
            {
                return _stereotype.GetProperty<bool>("Is Collection Enabled");
            }

            public bool IsNavigableDefault()
            {
                return _stereotype.GetProperty<bool>("Is Navigable Default");
            }

            public bool IsNullableDefault()
            {
                return _stereotype.GetProperty<bool>("Is Nullable Default");
            }

            public bool IsCollectionDefault()
            {
                return _stereotype.GetProperty<bool>("Is Collection Default");
            }

            public bool AllowMultiple()
            {
                return _stereotype.GetProperty<bool>("Allow Multiple");
            }

            public bool AllowSorting()
            {
                return _stereotype.GetProperty<bool>("Allow Sorting");
            }

            public SortChildrenOptions SortChildren()
            {
                return new SortChildrenOptions(_stereotype.GetProperty<string>("Sort Children"));
            }

            public class NameAccessibilityOptions
            {
                public readonly string Value;

                public NameAccessibilityOptions(string value)
                {
                    Value = value;
                }

                public NameAccessibilityOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Optional":
                            return NameAccessibilityOptionsEnum.Optional;
                        case "Required":
                            return NameAccessibilityOptionsEnum.Required;
                        case "Disabled":
                            return NameAccessibilityOptionsEnum.Disabled;
                        case "Hidden":
                            return NameAccessibilityOptionsEnum.Hidden;
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
                public bool IsDisabled()
                {
                    return Value == "Disabled";
                }
                public bool IsHidden()
                {
                    return Value == "Hidden";
                }
            }

            public enum NameAccessibilityOptionsEnum
            {
                Optional,
                Required,
                Disabled,
                Hidden
            }

            public class SortChildrenOptions
            {
                public readonly string Value;

                public SortChildrenOptions(string value)
                {
                    Value = value;
                }

                public SortChildrenOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Manually":
                            return SortChildrenOptionsEnum.Manually;
                        case "By type then manually":
                            return SortChildrenOptionsEnum.ByTypeThenManually;
                        case "By type then name":
                            return SortChildrenOptionsEnum.ByTypeThenName;
                        case "By name":
                            return SortChildrenOptionsEnum.ByName;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsManually()
                {
                    return Value == "Manually";
                }
                public bool IsByTypeThenManually()
                {
                    return Value == "By type then manually";
                }
                public bool IsByTypeThenName()
                {
                    return Value == "By type then name";
                }
                public bool IsByName()
                {
                    return Value == "By name";
                }
            }

            public enum SortChildrenOptionsEnum
            {
                Manually,
                ByTypeThenManually,
                ByTypeThenName,
                ByName
            }

        }

    }
}