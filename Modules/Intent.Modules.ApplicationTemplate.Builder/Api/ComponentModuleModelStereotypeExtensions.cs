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
    public static class ComponentModuleModelStereotypeExtensions
    {
        public static ModuleSettings GetModuleSettings(this ComponentModuleModel model)
        {
            var stereotype = model.GetStereotype(ModuleSettings.DefinitionId);
            return stereotype != null ? new ModuleSettings(stereotype) : null;
        }

        public static bool HasModuleSettings(this ComponentModuleModel model)
        {
            return model.HasStereotype(ModuleSettings.DefinitionId);
        }

        public static bool TryGetModuleSettings(this ComponentModuleModel model, out ModuleSettings stereotype)
        {
            if (!HasModuleSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ModuleSettings(model.GetStereotype(ModuleSettings.DefinitionId));
            return true;
        }


        public class ModuleSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7033e6f4-2a22-4357-bd65-f0ec06c516d5";

            public ModuleSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Version()
            {
                return _stereotype.GetProperty<string>("Version");
            }

            public bool IncludeByDefault()
            {
                return _stereotype.GetProperty<bool>("Include By Default");
            }

            public bool IsRequired()
            {
                return _stereotype.GetProperty<bool>("Is Required");
            }

            public IncludeAssetsOptions IncludeAssets()
            {
                return new IncludeAssetsOptions(_stereotype.GetProperty<string>("Include Assets"));
            }

            public IncludedAssetsOptions[] IncludedAssets()
            {
                return _stereotype.GetProperty<string[]>("Included Assets")?.Select(x => new IncludedAssetsOptions(x)).ToArray() ?? new IncludedAssetsOptions[0];
            }

            public class IncludeAssetsOptions
            {
                public readonly string Value;

                public IncludeAssetsOptions(string value)
                {
                    Value = value;
                }

                public IncludeAssetsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "All":
                            return IncludeAssetsOptionsEnum.All;
                        case "None":
                            return IncludeAssetsOptionsEnum.None;
                        case "Select":
                            return IncludeAssetsOptionsEnum.Select;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAll()
                {
                    return Value == "All";
                }
                public bool IsNone()
                {
                    return Value == "None";
                }
                public bool IsSelect()
                {
                    return Value == "Select";
                }
            }

            public enum IncludeAssetsOptionsEnum
            {
                All,
                None,
                Select
            }
            public class IncludedAssetsOptions
            {
                public readonly string Value;

                public IncludedAssetsOptions(string value)
                {
                    Value = value;
                }

                public IncludedAssetsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Application Settings":
                            return IncludedAssetsOptionsEnum.ApplicationSettings;
                        case "Designer Metadata":
                            return IncludedAssetsOptionsEnum.DesignerMetadata;
                        case "Designers":
                            return IncludedAssetsOptionsEnum.Designers;
                        case "Factory Extensions":
                            return IncludedAssetsOptionsEnum.FactoryExtensions;
                        case "Template Outputs":
                            return IncludedAssetsOptionsEnum.TemplateOutputs;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsApplicationSettings()
                {
                    return Value == "Application Settings";
                }
                public bool IsDesignerMetadata()
                {
                    return Value == "Designer Metadata";
                }
                public bool IsDesigners()
                {
                    return Value == "Designers";
                }
                public bool IsFactoryExtensions()
                {
                    return Value == "Factory Extensions";
                }
                public bool IsTemplateOutputs()
                {
                    return Value == "Template Outputs";
                }
            }

            public enum IncludedAssetsOptionsEnum
            {
                ApplicationSettings,
                DesignerMetadata,
                Designers,
                FactoryExtensions,
                TemplateOutputs
            }

        }

    }
}