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
    public static class ElementSettingsModelStereotypeExtensions
    {
        public static Settings GetSettings(this ElementSettingsModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this ElementSettingsModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this ElementSettingsModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype(Settings.DefinitionId));
            return true;
        }

        public static TypeReferenceSettings GetTypeReferenceSettings(this ElementSettingsModel model)
        {
            var stereotype = model.GetStereotype(TypeReferenceSettings.DefinitionId);
            return stereotype != null ? new TypeReferenceSettings(stereotype) : null;
        }

        public static bool HasTypeReferenceSettings(this ElementSettingsModel model)
        {
            return model.HasStereotype(TypeReferenceSettings.DefinitionId);
        }

        public static bool TryGetTypeReferenceSettings(this ElementSettingsModel model, out TypeReferenceSettings stereotype)
        {
            if (!HasTypeReferenceSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TypeReferenceSettings(model.GetStereotype(TypeReferenceSettings.DefinitionId));
            return true;
        }


        public class Settings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f406222b-31a8-435e-80f6-6a08f9108649";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SaveModeOptions SaveMode()
            {
                return new SaveModeOptions(_stereotype.GetProperty<string>("Save Mode"));
            }

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public IIconModel ExpandedIcon()
            {
                return _stereotype.GetProperty<IIconModel>("Expanded Icon");
            }

            public string IconFunction()
            {
                return _stereotype.GetProperty<string>("Icon Function");
            }

            public string DisplayTextFunction()
            {
                return _stereotype.GetProperty<string>("Display Text Function");
            }

            public bool NameMustBeUnique()
            {
                return _stereotype.GetProperty<bool>("Name Must Be Unique");
            }

            public string ValidateFunction()
            {
                return _stereotype.GetProperty<string>("Validate Function");
            }

            public bool AllowRename()
            {
                return _stereotype.GetProperty<bool>("Allow Rename");
            }

            public bool AllowAbstract()
            {
                return _stereotype.GetProperty<bool>("Allow Abstract");
            }

            public bool AllowStatic()
            {
                return _stereotype.GetProperty<bool>("Allow Static");
            }

            public bool AllowSetValue()
            {
                return _stereotype.GetProperty<bool>("Allow Set Value");
            }

            public bool AllowGenericTypes()
            {
                return _stereotype.GetProperty<bool>("Allow Generic Types");
            }

            public bool AllowSorting()
            {
                return _stereotype.GetProperty<bool>("Allow Sorting");
            }

            public SortChildrenOptions SortChildren()
            {
                return new SortChildrenOptions(_stereotype.GetProperty<string>("Sort Children"));
            }

            public bool AllowFindInView()
            {
                return _stereotype.GetProperty<bool>("Allow Find in View");
            }

            public class SaveModeOptions
            {
                public readonly string Value;

                public SaveModeOptions(string value)
                {
                    Value = value;
                }

                public SaveModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return SaveModeOptionsEnum.Default;
                        case "Own File":
                            return SaveModeOptionsEnum.OwnFile;
                        case "As Child":
                            return SaveModeOptionsEnum.AsChild;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsOwnFile()
                {
                    return Value == "Own File";
                }
                public bool IsAsChild()
                {
                    return Value == "As Child";
                }
            }

            public enum SaveModeOptionsEnum
            {
                Default,
                OwnFile,
                AsChild
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

        public class TypeReferenceSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d8f6f331-d4f0-4c75-aa91-f2e715cd9591";

            public TypeReferenceSettings(IStereotype stereotype)
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

            public RepresentsOptions Represents()
            {
                return new RepresentsOptions(_stereotype.GetProperty<string>("Represents"));
            }

            public string DefaultTypeId()
            {
                return _stereotype.GetProperty<string>("Default Type Id");
            }

            public bool AllowNullable()
            {
                return _stereotype.GetProperty<bool>("Allow Nullable");
            }

            public bool AllowCollection()
            {
                return _stereotype.GetProperty<bool>("Allow Collection");
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
                        case "Disabled":
                            return ModeOptionsEnum.Disabled;
                        case "Optional":
                            return ModeOptionsEnum.Optional;
                        case "Required":
                            return ModeOptionsEnum.Required;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDisabled()
                {
                    return Value == "Disabled";
                }
                public bool IsOptional()
                {
                    return Value == "Optional";
                }
                public bool IsRequired()
                {
                    return Value == "Required";
                }
            }

            public enum ModeOptionsEnum
            {
                Disabled,
                Optional,
                Required
            }
            public class RepresentsOptions
            {
                public readonly string Value;

                public RepresentsOptions(string value)
                {
                    Value = value;
                }

                public RepresentsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Reference":
                            return RepresentsOptionsEnum.Reference;
                        case "Inheritance":
                            return RepresentsOptionsEnum.Inheritance;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsReference()
                {
                    return Value == "Reference";
                }
                public bool IsInheritance()
                {
                    return Value == "Inheritance";
                }
            }

            public enum RepresentsOptionsEnum
            {
                Reference,
                Inheritance
            }
        }

    }
}