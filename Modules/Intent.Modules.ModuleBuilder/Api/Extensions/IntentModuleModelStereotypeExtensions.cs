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
    public static class IntentModuleModelStereotypeExtensions
    {
        public static ModuleSettings GetModuleSettings(this IntentModuleModel model)
        {
            var stereotype = model.GetStereotype(ModuleSettings.DefinitionId);
            return stereotype != null ? new ModuleSettings(stereotype) : null;
        }

        public static bool HasModuleSettings(this IntentModuleModel model)
        {
            return model.HasStereotype(ModuleSettings.DefinitionId);
        }

        public static bool TryGetModuleSettings(this IntentModuleModel model, out ModuleSettings stereotype)
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
            public const string DefinitionId = "76256322-88f1-4efb-8bd6-7cc4a6c54cb9";

            public ModuleSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Version()
            {
                return _stereotype.GetProperty<string>("Version");
            }

            public string APINamespace()
            {
                return _stereotype.GetProperty<string>("API Namespace");
            }

            public string NuGetPackageId()
            {
                return _stereotype.GetProperty<string>("NuGet Package Id");
            }

            public string NuGetPackageVersion()
            {
                return _stereotype.GetProperty<string>("NuGet Package Version");
            }

            public bool IncludeInModule()
            {
                return _stereotype.GetProperty<bool>("Include in Module");
            }

            public ReferenceInOptions ReferenceIn()
            {
                return new ReferenceInOptions(_stereotype.GetProperty<string>("Reference in"));
            }

            public IElement[] ReferenceInDesigner()
            {
                return _stereotype.GetProperty<IElement[]>("Reference in Designer") ?? new IElement[0];
            }

            public bool IncludeReleaseNotes()
            {
                return _stereotype.GetProperty<bool>("Include Release Notes");
            }

            public string ProjectURL()
            {
                return _stereotype.GetProperty<string>("Project URL");
            }

            public class ReferenceInOptions
            {
                public readonly string Value;

                public ReferenceInOptions(string value)
                {
                    Value = value;
                }

                public ReferenceInOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "All Designers":
                            return ReferenceInOptionsEnum.AllDesigners;
                        case "Selected Designers":
                            return ReferenceInOptionsEnum.SelectedDesigners;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAllDesigners()
                {
                    return Value == "All Designers";
                }
                public bool IsSelectedDesigners()
                {
                    return Value == "Selected Designers";
                }
            }

            public enum ReferenceInOptionsEnum
            {
                AllDesigners,
                SelectedDesigners
            }

        }

    }
}