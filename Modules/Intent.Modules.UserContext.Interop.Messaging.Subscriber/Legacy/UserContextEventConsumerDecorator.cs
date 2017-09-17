using Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.UserContext.Interop.Messaging.Subscriber.Legacy
{
    public class UserContextEventConsumerDecorator : IEventConsumerDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.UserContext.Interop.Messaging.Subscriber.Decorator.Legacy";

        public IEnumerable<string> DeclareUsings() => new[]
        {
            "using Intent.Framework;",
            "using Intent.Framework.Core.Context;",
        };

        public string DeclarePrivateVariables() => @"
        private readonly IUserContextProvider<IUserContextData> _userContextProvider;";

        public string ConstructorParams() => @"
            , IUserContextProvider<IUserContextData> userContextProvider";

        public string ConstructorInit() => @"
            _userContextProvider = userContextProvider;";

        public string BeginOperation() => @"
            var userContext = _userContextProvider.GetUserContext();
            ServiceCallContext.Instance.Set<IUserContextData>(userContext);";

        public string BeforeTransaction() => @"";

        public string BeforeCallToAppLayer() => @"";

        public string AfterCallToAppLayer() => @"";

        public string AfterTransaction() => @"";

        public string OnExceptionCaught() => @"";

        public bool HandlesCaughtException() => false;

        public string ClassMethods() => @"";

        public int Priority { get; set; } = -200;

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}