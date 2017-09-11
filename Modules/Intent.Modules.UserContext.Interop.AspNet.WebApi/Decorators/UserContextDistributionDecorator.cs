using System.Collections.Generic;
using System.Data.Common;
using Intent.MetaModel.Service;
using Intent.Packages.UserContext.Templates.UserContextInterface;
using Intent.Modules.WebApi.Decorators;
using Intent.Modules.WebApi.Legacy;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.UserContext.Interop.WebApi.Decorators
{
    public class UserContextDistributionDecorator : DistributionDecoratorBase, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.UserContext.Interop.WebApi.DistributionDecorator";

        public override IEnumerable<string> DeclareUsings(IServiceModel service)
        {
            return new[]
            {
                "using Intent.Framework;",
                "using Intent.Framework.Core.Context;",
            };
        }

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private readonly IUserContextProvider<IUserContextData> _userContextProvider;";

        public override string ConstructorParams(IServiceModel service) => @"
            , IUserContextProvider<IUserContextData> userContextProvider";

        public override string ConstructorInit(IServiceModel service) => @"
            _userContextProvider = userContextProvider;";

        public override string BeginOperation(IServiceModel service, IOperationModel operation) => @"
            var userContext = _userContextProvider.GetUserContext();
            ServiceCallContext.Instance.Set<IUserContextData>(userContext);";

        public override int Priority { get; set; } = -200;

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}