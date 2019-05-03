using Intent.Modelers.Services.Api;

namespace Intent.Modules.Angular.Api
{
    public interface IModuleDTOModel : IDTOModel
    {
        IModuleModel Module { get; }
    }
}