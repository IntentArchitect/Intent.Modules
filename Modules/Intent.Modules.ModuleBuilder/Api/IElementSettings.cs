using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IElementSettings : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        IContextMenu MenuOptions { get; }
        IList<IAttributeSettings> AttributeSettings { get; }
        IList<IDiagramSettings> DiagramSettings { get; }
        IList<ILiteralSettings> LiteralSettings { get; }
        IList<IMappingSettings> MappingSettings { get; }
        IList<IOperationSettings> OperationSettings { get; }
        IList<IElementSettings> ChildElementSettings { get; }

        [IntentManaged(Mode.Ignore)]
        IModeler Modeler { get; }

        [IntentManaged(Mode.Ignore)]
        bool IsChild { get; }
    }
}