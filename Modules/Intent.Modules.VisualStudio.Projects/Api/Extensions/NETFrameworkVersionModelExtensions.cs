using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class NETFrameworkVersionModelExtensions
    {
        public static NETFrameworkVersionSettings GetNETFrameworkVersionSettings(this NETFrameworkVersionModel model)
        {
            var stereotype = model.GetStereotype(".NET Framework Version Settings");
            return stereotype != null ? new NETFrameworkVersionSettings(stereotype) : null;
        }

        public static bool HasNETFrameworkVersionSettings(this NETFrameworkVersionModel model)
        {
            return model.HasStereotype(".NET Framework Version Settings");
        }


        public class NETFrameworkVersionSettings
        {
            private IStereotype _stereotype;

            public NETFrameworkVersionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string LegacyVersionIdentifier()
            {
                return _stereotype.GetProperty<string>("Legacy Version Identifier");
            }

        }

    }
}