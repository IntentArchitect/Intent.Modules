using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ICreationOptionModel
    {
        IIconModel Icon { get; }
        bool AllowMultiple();
        ElementCreationOption ToPersistable();
    }
}