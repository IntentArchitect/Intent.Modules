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
            var stereotype = model.GetStereotype("Settings");
            return stereotype != null ? new Settings(stereotype) : null;
        }

        public static bool HasSettings(this AssociationSourceEndSettingsModel model)
        {
            return model.HasStereotype("Settings");
        }

        public static bool TryGetSettings(this AssociationSourceEndSettingsModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype("Settings"));
            return true;
        }


        public class Settings
        {
            private IStereotype _stereotype;

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

            public string DisplayTextFunction()
            {
                return _stereotype.GetProperty<string>("Display Text Function");
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