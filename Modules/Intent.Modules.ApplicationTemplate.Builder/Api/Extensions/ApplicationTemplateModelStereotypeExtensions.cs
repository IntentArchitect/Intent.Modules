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
            var stereotype = model.GetStereotype("Application Template Defaults");
            return stereotype != null ? new ApplicationTemplateDefaults(stereotype) : null;
        }


        public static bool HasApplicationTemplateDefaults(this ApplicationTemplateModel model)
        {
            return model.HasStereotype("Application Template Defaults");
        }
        public static ApplicationTemplateSettings GetApplicationTemplateSettings(this ApplicationTemplateModel model)
        {
            var stereotype = model.GetStereotype("Application Template Settings");
            return stereotype != null ? new ApplicationTemplateSettings(stereotype) : null;
        }

        public static bool HasApplicationTemplateSettings(this ApplicationTemplateModel model)
        {
            return model.HasStereotype("Application Template Settings");
        }


        public class ApplicationTemplateDefaults
        {
            private IStereotype _stereotype;

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

            public bool StoreIntentArchitectFilesSeparateToCodebase()
            {
                return _stereotype.GetProperty<bool>("Store Intent Architect files separate to codebase");
            }

        }


        public class ApplicationTemplateSettings
        {
            private IStereotype _stereotype;

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