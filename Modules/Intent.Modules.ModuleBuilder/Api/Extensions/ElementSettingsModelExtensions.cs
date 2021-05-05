using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ElementSettingsModelExtensions
    {
        public static Settings GetSettings(this ElementSettingsModel model)
        {
            var stereotype = model.GetStereotype("Settings");
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this ElementSettingsModel model)
        {
            return model.HasStereotype("Settings");
        }

        public static TypeReferenceSettings GetTypeReferenceSettings(this ElementSettingsModel model)
        {
            var stereotype = model.GetStereotype("Type Reference Settings");
            return stereotype != null ? new TypeReferenceSettings(stereotype) : null;
        }

        public static bool HasTypeReferenceSettings(this ElementSettingsModel model)
        {
            return model.HasStereotype("Type Reference Settings");
        }


        public class Settings
        {
            private IStereotype _stereotype;

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

            public string DisplayTextFunction()
            {
                return _stereotype.GetProperty<string>("Display Text Function");
            }

            public bool AllowRename()
            {
                return _stereotype.GetProperty<bool>("Allow Rename");
            }

            public bool AllowAbstract()
            {
                return _stereotype.GetProperty<bool>("Allow Abstract");
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

            public class SortChildrenOptions
            {
                public readonly string Value;

                public SortChildrenOptions(string value)
                {
                    Value = value;
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

        }

        public class TypeReferenceSettings
        {
            private IStereotype _stereotype;

            public TypeReferenceSettings(IStereotype stereotype)
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

            public class RepresentsOptions
            {
                public readonly string Value;

                public RepresentsOptions(string value)
                {
                    Value = value;
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

        }

    }
}