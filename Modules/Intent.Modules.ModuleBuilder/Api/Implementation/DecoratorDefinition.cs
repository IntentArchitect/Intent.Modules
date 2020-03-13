using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class DecoratorDefinition : ModuleBuilderElementBase, IDecoratorDefinition
    {
        public DecoratorDefinition(IElement element) : base(element)
        {
        }
    }
}