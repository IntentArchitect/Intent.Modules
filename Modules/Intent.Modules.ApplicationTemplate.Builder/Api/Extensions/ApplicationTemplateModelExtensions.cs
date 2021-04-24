using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ApplicationTemplate.Builder.Api
{
    public static class ApplicationTemplateModelExtensions
    {
        public static ApplicationTemplateSettings GetApplicationTemplateSettings(this ApplicationTemplateModel model)
        {
            var stereotype = model.GetStereotype("Application Template Settings");
            return stereotype != null ? new ApplicationTemplateSettings(stereotype) : null;
        }

        public static bool HasApplicationTemplateSettings(this ApplicationTemplateModel model)
        {
            return model.HasStereotype("Application Template Settings");
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

            public string DefaultName()
            {
                return _stereotype.GetProperty<string>("Default Name");
            }

            public string DefaultOutputLocation()
            {
                return _stereotype.GetProperty<string>("Default Output Location");
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