using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Api
{
    public interface IFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IList<ICSharpTemplate> CSharpTemplates { get; }
    }
}