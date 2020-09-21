using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Interop.AspNet.WebApi.Decorators
{
    public class UserContextWebApiControllerDecorator : WebApiControllerDecoratorBase, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.UserContext.Interop.WebApi.DistributionDecorator";

        public override IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Intent.Framework",
                "Intent.Framework.Core.Context",
            };
        }

        public override string DeclarePrivateVariables(ServiceModel service) => @"
        private readonly IUserContextProvider<IUserContextData> _userContextProvider;";

        public override string ConstructorParams(ServiceModel service) => @"
            , IUserContextProvider<IUserContextData> userContextProvider";

        public override string ConstructorInit(ServiceModel service) => @"
            _userContextProvider = userContextProvider;";

        public override string BeginOperation(ServiceModel service, OperationModel operation) => @"
            var userContext = _userContextProvider.GetUserContext();
            ServiceCallContext.Instance.Set<IUserContextData>(userContext);";

        public override int Priority { get; set; } = -200;

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}