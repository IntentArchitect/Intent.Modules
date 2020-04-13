using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelExtensions", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class MappingOutputExtensions
    {
        public static OutputSettings GetOutputSettings(this MappingOutputModel model)
        {
            var stereotype = model.GetStereotype("Output Settings");
            return stereotype != null ? new OutputSettings(stereotype) : null;
        }


        public class OutputSettings
        {
            private IStereotype _stereotype;

            public OutputSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

        }

    }
}