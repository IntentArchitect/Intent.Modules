using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;

namespace Intent.ModuleBuilder.Api
{
    public interface ICreationOptionModel
    {
        IIconModel Icon { get; }
        bool AllowMultiple();
        ContextMenuOption ToPersistable();
    }
}