using System;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory.Engine;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.DecoratorTemplate", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Decorators.CompositeDecorator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CompositeDecorator : ModuleTests.ModuleBuilderTests.Templates.Composite.ICompositeContract
    {
        public const string Identifier = "ModuleBuilderTests.CompositeDecorator";

        private readonly IApplication _application;

        public CompositeDecorator(IApplication application)
        {
            _application = application;
        }

        public string GetDecoratorText()
        {
            return "This is text from my Test Decorator";
        }
    }
}