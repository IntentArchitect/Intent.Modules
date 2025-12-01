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
    public static class ComponentModelStereotypeExtensions
    {
        public static ComponentSettings GetComponentSettings(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(ComponentSettings.DefinitionId);
            return stereotype != null ? new ComponentSettings(stereotype) : null;
        }

        public static bool HasComponentSettings(this ComponentModel model)
        {
            return model.HasStereotype(ComponentSettings.DefinitionId);
        }

        public static bool TryGetComponentSettings(this ComponentModel model, out ComponentSettings stereotype)
        {
            if (!HasComponentSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ComponentSettings(model.GetStereotype(ComponentSettings.DefinitionId));
            return true;
        }


        public class ComponentSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "78da9b5b-428f-4b1e-a5e5-120c7b3d87d3";

            public ComponentSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string Description()
            {
                return _stereotype.GetProperty<string>("Description");
            }

            public bool IncludeByDefault()
            {
                return _stereotype.GetProperty<bool>("Include by Default");
            }

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public bool IsNew()
            {
                return _stereotype.GetProperty<bool>("Is New");
            }

            public IElement[] Dependencies()
            {
                return _stereotype.GetProperty<IElement[]>("Dependencies") ?? new IElement[0];
            }

            public IElement[] Incompatibilities()
            {
                return _stereotype.GetProperty<IElement[]>("Incompatibilities") ?? new IElement[0];
            }

            public RequiredLicenseOptions RequiredLicense()
            {
                return new RequiredLicenseOptions(_stereotype.GetProperty<string>("Required License"));
            }

            public string DocumentationUrl()
            {
                return _stereotype.GetProperty<string>("Documentation Url");
            }

            public string Tags()
            {
                return _stereotype.GetProperty<string>("Tags");
            }

            public class RequiredLicenseOptions
            {
                public readonly string Value;

                public RequiredLicenseOptions(string value)
                {
                    Value = value;
                }

                public RequiredLicenseOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "None":
                            return RequiredLicenseOptionsEnum.None;
                        case "Professional":
                            return RequiredLicenseOptionsEnum.Professional;
                        case "Premium":
                            return RequiredLicenseOptionsEnum.Premium;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsProfessional()
                {
                    return Value == "Professional";
                }
                public bool IsPremium()
                {
                    return Value == "Premium";
                }
            }

            public enum RequiredLicenseOptionsEnum
            {
                None,
                Professional,
                Premium
            }

        }

    }
}