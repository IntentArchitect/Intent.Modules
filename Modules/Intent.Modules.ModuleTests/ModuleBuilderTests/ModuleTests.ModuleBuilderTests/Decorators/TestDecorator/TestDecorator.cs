using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.DecoratorTemplate", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Decorators.TestDecorator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TestDecorator : ModuleTests.ModuleBuilderTests.Templates.Composite.ICompositeContract
    {
        public string GetDecoratorText()
        {
            return "This is text from my Test Decorator";
        }
    }
}