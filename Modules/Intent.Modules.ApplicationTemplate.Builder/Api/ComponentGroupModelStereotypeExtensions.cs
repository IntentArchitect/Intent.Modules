using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ComponentGroupModelStereotypeExtensions
    {
        public static ComponentGroupSettings GetComponentGroupSettings(this ComponentGroupModel model)
        {
            var stereotype = model.GetStereotype("08b4c68f-3654-44cc-b3a1-87de79c23936");
            return stereotype != null ? new ComponentGroupSettings(stereotype) : null;
        }


        public static bool HasComponentGroupSettings(this ComponentGroupModel model)
        {
            return model.HasStereotype("08b4c68f-3654-44cc-b3a1-87de79c23936");
        }

        public static bool TryGetComponentGroupSettings(this ComponentGroupModel model, out ComponentGroupSettings stereotype)
        {
            if (!HasComponentGroupSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ComponentGroupSettings(model.GetStereotype("08b4c68f-3654-44cc-b3a1-87de79c23936"));
            return true;
        }


        public class ComponentGroupSettings
        {
            private IStereotype _stereotype;

            public ComponentGroupSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SelectionModeOptions SelectionMode()
            {
                return new SelectionModeOptions(_stereotype.GetProperty<string>("Selection Mode"));
            }

            public class SelectionModeOptions
            {
                public readonly string Value;

                public SelectionModeOptions(string value)
                {
                    Value = value;
                }

                public SelectionModeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Allow Multiple":
                            return SelectionModeOptionsEnum.AllowMultiple;
                        case "Allow Single Only":
                            return SelectionModeOptionsEnum.AllowSingleOnly;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAllowMultiple()
                {
                    return Value == "Allow Multiple";
                }
                public bool IsAllowSingleOnly()
                {
                    return Value == "Allow Single Only";
                }
            }

            public enum SelectionModeOptionsEnum
            {
                AllowMultiple,
                AllowSingleOnly
            }
        }

    }
}