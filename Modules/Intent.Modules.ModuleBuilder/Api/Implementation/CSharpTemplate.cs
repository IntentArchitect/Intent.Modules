using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    class CSharpTemplate : ModuleBuilderElementBase, ICSharpTemplate
    {
        public CSharpTemplate(IElement element) : base(element)
        {
        }
    }
}