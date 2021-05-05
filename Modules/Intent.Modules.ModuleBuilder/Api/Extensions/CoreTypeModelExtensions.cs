using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class CoreTypeModelExtensions
    {
        public static Settings GetSettings(this CoreTypeModel model)
        {
            var stereotype = model.GetStereotype("Settings");
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this CoreTypeModel model)
        {
            return model.HasStereotype("Settings");
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

    }
}