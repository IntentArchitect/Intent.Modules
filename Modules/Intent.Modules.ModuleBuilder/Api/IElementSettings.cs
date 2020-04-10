using System;
using System.Collections.Generic;
using Intent.IArchitect.Agent.Persistence.Model.Common;
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
        IList<IDiagramSettings> DiagramSettings { get; }
        IMappingSettings MappingSettings { get; }
        IList<IElementSettings> ChildElementSettings { get; }

        [IntentManaged(Mode.Ignore)]
        IModeler Modeler { get; }

        [IntentManaged(Mode.Ignore)]
        bool IsChild { get; }

        [IntentManaged(Mode.Ignore)]
        ElementSettingsPersistable ToPersistable();
    }
}