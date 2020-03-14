using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IAssociationSettings : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IAssociationEndSettings SourceEnd { get; }
        IAssociationEndSettings DestinationEnd { get; }
    }
}