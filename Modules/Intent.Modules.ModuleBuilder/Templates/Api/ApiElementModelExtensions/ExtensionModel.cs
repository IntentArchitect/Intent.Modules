using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    public class ExtensionModel
    {
        public ExtensionModel(ExtensionModelType type, IEnumerable<IStereotypeDefinition> stereotypeDefinitions)
        {
            StereotypeDefinitions = stereotypeDefinitions;
            Type = type;
        }

        public IEnumerable<IStereotypeDefinition> StereotypeDefinitions { get; }

        public ExtensionModelType Type { get; set; }
    }
}