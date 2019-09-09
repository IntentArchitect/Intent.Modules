using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ICSharpTemplate : IModuleBuilderElement
    {
    }

    public class CSharpTemplate : ModuleBuilderElementBase, ICSharpTemplate
    {
        public CSharpTemplate(IElement element) : base(element)
        {
        }
    }
}
