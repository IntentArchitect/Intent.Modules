using Intent.Metadata.Models;

namespace Intent.Modules.Angular.Api
{
    public interface IComponentModel : IHasStereotypes, IMetaModel
    {
        string Name { get; }
        string Comment { get; }
        IModuleModel Module { get; }
    }
}