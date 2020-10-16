using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNet.SignalR.Templates.ClientNotificationService;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.AspNet.SignalR.Interop.WebApi.Decorator
{
    internal class SignalrWebApiWebApiControllerDecorator : WebApiControllerDecoratorBase
    {
        public const string Identifier = "Intent.AspNet.SignalR.Interop.WebApi";

        public SignalrWebApiWebApiControllerDecorator()
        {
            
        }

        public override string DeclarePrivateVariables(ServiceModel service)
        {
            return @"
        private readonly IClientNotificationService _clientNotificationService;";
        }

        public override string ConstructorParams(ServiceModel service)
        {
            return @"
            , IClientNotificationService clientNotificationService";
        }

        public override string ConstructorInit(ServiceModel service)
        {
            return @"
            _clientNotificationService = clientNotificationService;";
        }

        public override string AfterTransaction(ServiceModel service, OperationModel operation)
        {
            return @"
                _clientNotificationService.Flush();";
        }

        public override int Priority { get; set; } = -500;

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(ClientNotificationService.Identifier),
            };
        }
    }
}
