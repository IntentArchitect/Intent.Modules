using Intent.Templates;
using System;

namespace ModuleTests.ModuleBuilderTests.Templates.Composite
{
    public interface ICompositeContract : ITemplateDecorator
    {
        string GetDecoratorText();
    }
}
