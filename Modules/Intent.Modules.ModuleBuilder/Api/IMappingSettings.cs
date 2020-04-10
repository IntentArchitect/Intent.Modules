using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;
using Intent.IArchitect.Agent.Persistence.Model.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IMappingSettings : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IList<IElementMapping> Mappings { get; }

        [IntentManaged(Mode.Ignore)]
        ElementMappingSettings ToPersistable();
    }
}