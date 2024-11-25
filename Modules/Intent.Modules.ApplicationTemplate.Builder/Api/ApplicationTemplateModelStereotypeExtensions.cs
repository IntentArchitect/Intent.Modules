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
    public static class ApplicationTemplateModelStereotypeExtensions
    {
        public static ApplicationTemplateDefaults GetApplicationTemplateDefaults(this ApplicationTemplateModel model)
        {
            var stereotype = model.GetStereotype(ApplicationTemplateDefaults.DefinitionId);
            return stereotype != null ? new ApplicationTemplateDefaults(stereotype) : null;
        }


        public static bool HasApplicationTemplateDefaults(this ApplicationTemplateModel model)
        {
            return model.HasStereotype(ApplicationTemplateDefaults.DefinitionId);
        }

        public static bool TryGetApplicationTemplateDefaults(this ApplicationTemplateModel model, out ApplicationTemplateDefaults stereotype)
        {
            if (!HasApplicationTemplateDefaults(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApplicationTemplateDefaults(model.GetStereotype(ApplicationTemplateDefaults.DefinitionId));
            return true;
        }
        public static ApplicationTemplateSettings GetApplicationTemplateSettings(this ApplicationTemplateModel model)
        {
            var stereotype = model.GetStereotype(ApplicationTemplateSettings.DefinitionId);
            return stereotype != null ? new ApplicationTemplateSettings(stereotype) : null;
        }

        public static bool HasApplicationTemplateSettings(this ApplicationTemplateModel model)
        {
            return model.HasStereotype(ApplicationTemplateSettings.DefinitionId);
        }

        public static bool TryGetApplicationTemplateSettings(this ApplicationTemplateModel model, out ApplicationTemplateSettings stereotype)
        {
            if (!HasApplicationTemplateSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApplicationTemplateSettings(model.GetStereotype(ApplicationTemplateSettings.DefinitionId));
            return true;
        }


        public class ApplicationTemplateDefaults
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "e99df2da-e5dd-41c0-bd03-c642cec089a8";

            public ApplicationTemplateDefaults(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string RelativeOutputLocation()
            {
                return _stereotype.GetProperty<string>("Relative Output Location");
            }

            public bool PlaceSolutionAndApplicationInTheSameDirectory()
            {
                return _stereotype.GetProperty<bool>("Place solution and application in the same directory");
            }

            public bool CreateFolderForSolution()
            {
                return _stereotype.GetProperty<bool>("Create folder for solution");
            }

            public bool StoreIntentArchitectFilesSeparateToCodebase()
            {
                return _stereotype.GetProperty<bool>("Store Intent Architect files separate to codebase");
            }

            public bool SetGitignoreEntries()
            {
                return _stereotype.GetProperty<bool>("Set .gitignore entries");
            }

        }


        public class ApplicationTemplateSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6e2b0cb0-3abd-4fce-b72e-d81c325a2632";

            public ApplicationTemplateSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string Version()
            {
                return _stereotype.GetProperty<string>("Version");
            }

            public string DisplayName()
            {
                return _stereotype.GetProperty<string>("Display Name");
            }

            public string Description()
            {
                return _stereotype.GetProperty<string>("Description");
            }

            public string Authors()
            {
                return _stereotype.GetProperty<string>("Authors");
            }

            public string Priority()
            {
                return _stereotype.GetProperty<string>("Priority");
            }

            public string SupportedClientVersions()
            {
                return _stereotype.GetProperty<string>("Supported Client Versions");
            }

        }

    }
}