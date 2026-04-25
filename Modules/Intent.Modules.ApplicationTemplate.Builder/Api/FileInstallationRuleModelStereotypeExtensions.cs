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
    public static class FileInstallationRuleModelStereotypeExtensions
    {
        public static FileInstallationRuleSettings GetFileInstallationRuleSettings(this FileInstallationRuleModel model)
        {
            var stereotype = model.GetStereotype(FileInstallationRuleSettings.DefinitionId);
            return stereotype != null ? new FileInstallationRuleSettings(stereotype) : null;
        }


        public static bool HasFileInstallationRuleSettings(this FileInstallationRuleModel model)
        {
            return model.HasStereotype(FileInstallationRuleSettings.DefinitionId);
        }

        public static bool TryGetFileInstallationRuleSettings(this FileInstallationRuleModel model, out FileInstallationRuleSettings stereotype)
        {
            if (!HasFileInstallationRuleSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileInstallationRuleSettings(model.GetStereotype(FileInstallationRuleSettings.DefinitionId));
            return true;
        }

        public class FileInstallationRuleSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c6ea8570-f40f-4f37-b192-426240926240";

            public FileInstallationRuleSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string MatchFiles()
            {
                return _stereotype.GetProperty<string>("Match Files");
            }

            public TargetOptions Target()
            {
                return new TargetOptions(_stereotype.GetProperty<string>("Target"));
            }

            public string RelativeOutputFolder()
            {
                return _stereotype.GetProperty<string>("Relative Output Folder");
            }

            public class TargetOptions
            {
                public readonly string Value;

                public TargetOptions(string value)
                {
                    Value = value;
                }

                public TargetOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Output Directory":
                            return TargetOptionsEnum.OutputDirectory;
                        case "Application Config Directory":
                            return TargetOptionsEnum.ApplicationConfigDirectory;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsOutputDirectory()
                {
                    return Value == "Output Directory";
                }
                public bool IsApplicationConfigDirectory()
                {
                    return Value == "Application Config Directory";
                }
            }

            public enum TargetOptionsEnum
            {
                OutputDirectory,
                ApplicationConfigDirectory
            }
        }

    }
}