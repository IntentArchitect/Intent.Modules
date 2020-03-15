using Intent.Metadata.Models;
using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ICreationOption
        : IMetadataModel, IHasStereotypes
    {
        string Text { get; }

        string Shortcut { get; }

        string DefaultName { get; }

        IconModel Icon { get; }

        IElement Type { get; }

        bool AllowMultiple { get; }

        string Name { get; }

        string TargetSpecializationType { get; }
    }
}