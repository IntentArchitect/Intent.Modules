using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IContextMenu : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IList<ICreationOption> CreationOptions { get; }
        IList<TypeOrder> TypeOrder { get; }
    }
}