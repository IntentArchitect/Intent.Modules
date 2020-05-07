using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public interface IModuleDTOModel : DTOModel
    {
        IModuleModel Module { get; }
    }
}