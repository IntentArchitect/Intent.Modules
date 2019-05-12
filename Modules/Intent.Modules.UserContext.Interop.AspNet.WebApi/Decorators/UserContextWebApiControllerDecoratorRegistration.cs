using Intent.Engine;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.UserContext.Interop.AspNet.WebApi.Decorators
{
    public class UserContextWebApiControllerDecoratorRegistration : DecoratorRegistration<WebApiControllerDecoratorBase>
    {
        public override string DecoratorId => UserContextWebApiControllerDecorator.Identifier;
        public override WebApiControllerDecoratorBase CreateDecoratorInstance(IApplication application)
        {
            return new UserContextWebApiControllerDecorator();
        }
    }
}