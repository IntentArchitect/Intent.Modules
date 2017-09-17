using Intent.Modules.AspNet.SignalR.Templates.ClientNotificationService;
using Intent.Modules.Messaging.Subscriber.Legacy.WebApiEventConsumerService;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.SignalR.Interop.Messaging.Subscriber.Decorators
{
    public class SignalrWebApiEventConsumerDistributionDecorator : IEventConsumerDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.AspNet.SignalR.Interop.Messaging.Subscriber";

        public string DeclarePrivateVariables() => @"
        private readonly IClientNotificationService _clientNotificationService;";

        public string ConstructorParams() => @"
            , IClientNotificationService clientNotificationService";

        public string ConstructorInit() => @"
            _clientNotificationService = clientNotificationService;";

        public string AfterTransaction() => @"
                _clientNotificationService.Flush();";

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(ClientNotificationService.Identifier),
            };
        }

        public int Priority() => 0;

        public IEnumerable<string> DeclareUsings() => new string[0];

        public string BeginOperation() => string.Empty;

        public string BeforeTransaction() => string.Empty;

        public string BeforeCallToAppLayer() => string.Empty;

        public string AfterCallToAppLayer() => string.Empty;

        public string OnExceptionCaught() => string.Empty;

        public bool HandlesCaughtException() => false;

        public string ClassMethods() => string.Empty;

        int IPriorityDecorator.Priority { get; set; } = 0;
    }
}