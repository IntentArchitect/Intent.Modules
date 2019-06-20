using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public class ModuleDTOModel : DTOModel, IModuleDTOModel
    {
        public ModuleDTOModel(IElement @class, IModuleModel module) : base(@class)
        {
            Module = module;
        }

        public IModuleModel Module { get; }
    }
}