using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModelerReference : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }
        IEnumerable<IModelerModelType> ModelTypes { get; }
        string ApiNamespace { get; }
        string ModuleDependency { get; }
        string ModuleVersion { get; }
        string NuGetDependency { get; }
        string NuGetVersion { get; }
    }
}