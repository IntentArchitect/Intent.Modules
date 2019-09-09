using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IDecoratorDefinition : IModuleBuilderElement
    {

    }

    public class DecoratorDefinition : ModuleBuilderElementBase, IDecoratorDefinition
    {
        public DecoratorDefinition(IElement element) : base(element)
        {
        }
    }
}