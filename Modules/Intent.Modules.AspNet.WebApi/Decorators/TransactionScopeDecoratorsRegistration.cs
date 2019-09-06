using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using System;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class BeginTransactionScopeDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => BeginTransactionScopeDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new BeginTransactionScopeDecorator(application);
        }
    }

    public class EndTransactionScopeDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => EndTransactionScopeDecorator.Identifier;

        public override object CreateDecoratorInstance(IApplication application)
        {
            return new EndTransactionScopeDecorator(application);
        }
    }
}
