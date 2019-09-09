using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IFileTemplate : IModuleBuilderElement
    {
    }

    public class FileTemplate : ModuleBuilderElementBase, IFileTemplate
    {
        public FileTemplate(IElement element) : base(element)
        {
        }
    }
}