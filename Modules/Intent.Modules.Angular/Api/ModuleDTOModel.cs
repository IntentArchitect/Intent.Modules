using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public class ModuleDTOModel : DTOModel
    {
        public ModuleDTOModel(IElement @class, ModuleModel module) : base(@class)
        {
            Module = module;
        }

        public ModuleModel Module { get; }
    }
}