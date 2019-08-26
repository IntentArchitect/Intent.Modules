using System;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.DecoratorRegistration.Template", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Decorators.CompositeDecorator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CompositeDecoratorRegistration : DecoratorRegistration<ModuleTests.ModuleBuilderTests.Templates.Composite.ICompositeContract>
    {
        public override string DecoratorId => ModuleTests.ModuleBuilderTests.Decorators.CompositeDecorator.CompositeDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new ModuleTests.ModuleBuilderTests.Decorators.CompositeDecorator.CompositeDecorator(application);
        }
    }
}