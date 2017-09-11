using System.Collections.Generic;
using Intent.Modules.WebApi.Legacy;
using Intent.Packages.UserContext.Templates.UserContextInterface;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.UserContext.Interop.WebApi.Legacy
{
    public class UserContextDistributionDecorator : BaseDistributionDecorator, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.UserContext.Interop.WebApi.DistributionDecorator.Legacy";

        public UserContextDistributionDecorator()
        {
            Priority = -200;
        }

        public override IEnumerable<string> DeclareUsings(ServiceModel service)
        {
            return new[]
            {
                "using Intent.Framework;",
                "using Intent.Framework.Core.Context;",
            };
        }

        public override string DeclarePrivateVariables(ServiceModel service) => @"
        private readonly IUserContextProvider<IUserContextData> _userContextProvider;";

        public override string ConstructorParams(ServiceModel service) => @"
            , IUserContextProvider<IUserContextData> userContextProvider";

        public override string ConstructorInit(ServiceModel service) => @"
            _userContextProvider = userContextProvider;";

        public override string BeginOperation(ServiceModel service, ServiceOperationModel operation) => @"
            var userContext = _userContextProvider.GetUserContext();
            ServiceCallContext.Instance.Set<IUserContextData>(userContext);";

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}