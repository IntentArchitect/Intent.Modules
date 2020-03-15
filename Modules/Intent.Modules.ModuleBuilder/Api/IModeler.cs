using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModeler : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IList<IElementSettings> ElementTypes { get; }
        IList<IAssociationSettings> AssociationTypes { get; }
        IPackageSettings PackageSettings { get; }
    }
}