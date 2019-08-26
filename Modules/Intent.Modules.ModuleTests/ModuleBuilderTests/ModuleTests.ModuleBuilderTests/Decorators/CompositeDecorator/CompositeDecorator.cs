using System;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.DecoratorTemplate", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Decorators.CompositeDecorator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CompositeDecorator : ModuleTests.ModuleBuilderTests.Templates.Composite.ICompositeContract, IDeclareUsings
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

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public IEnumerable<string> DeclareUsings()
        {
            return new string[]
            {
                // Specify list of Namespaces here, example:
                "System.Linq"
            };
        }
    }
}