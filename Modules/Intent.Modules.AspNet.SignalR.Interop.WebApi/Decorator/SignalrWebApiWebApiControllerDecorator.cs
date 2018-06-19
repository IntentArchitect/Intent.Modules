using System.Collections.Generic;
using Intent.MetaModel.Service;
using Intent.Modules.AspNet.SignalR.Templates.ClientNotificationService;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator
{
    internal class SignalrWebApiWebApiControllerDecorator : WebApiControllerDecoratorBase
    {
        public const string Identifier = "Intent.AspNet.SignalR.Interop.WebApi";

        public SignalrWebApiWebApiControllerDecorator()
        {
            
        }

        public override string DeclarePrivateVariables(IServiceModel service)
        {
            return @"
        private readonly IClientNotificationService _clientNotificationService;";
        }

        public override string ConstructorParams(IServiceModel service)
        {
            return @"
            , IClientNotificationService clientNotificationService";
        }

        public override string ConstructorInit(IServiceModel service)
        {
            return @"
            _clientNotificationService = clientNotificationService;";
        }

        public override string AfterTransaction(IServiceModel service, IOperationModel operation)
        {
            return @"
                _clientNotificationService.Flush();";
        }

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
