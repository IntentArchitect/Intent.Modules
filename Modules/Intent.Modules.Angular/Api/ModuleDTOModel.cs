using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public class ModuleDTOModel : DTOModel, IModuleDTOModel
    {
        public ModuleDTOModel(IClass @class, IModuleModel module) : base(@class)
        {
            Module = module;
        }

        public IModuleModel Module { get; }
    }
}