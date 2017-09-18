using System.Collections.Generic;
using Intent.Modules.AspNet.SignalR.Templates.ClientNotificationService;
using Intent.Modules.AspNet.WebApi.Legacy;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator
{
    internal class SignalrWebApiDistributionDecorator : BaseDistributionDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.AspNet.SignalR.Interop.WebApi.Legacy";

        public override string DeclarePrivateVariables(ServiceModel service) => @"
        private readonly IClientNotificationService _clientNotificationService;";

        public override string ConstructorParams(ServiceModel service) => @"
            , IClientNotificationService clientNotificationService";

        public override string ConstructorInit(ServiceModel service) => @"
            _clientNotificationService = clientNotificationService;";

        public override string AfterTransaction(ServiceModel service, ServiceOperationModel operation) => @"
                _clientNotificationService.Flush();";

        public override int Priority { get; set; } = -500;

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(ClientNotificationService.Identifier),
            };
        }
    }
}
