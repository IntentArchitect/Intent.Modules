using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using System;

namespace Intent.Modules.AspNet.WebApi.Decorators
{
    public class TransactionScopeDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => throw new NotImplementedException();

        public override object CreateDecoratorInstance(IApplication application)
        {
            throw new NotImplementedException();
        }
    }
}
