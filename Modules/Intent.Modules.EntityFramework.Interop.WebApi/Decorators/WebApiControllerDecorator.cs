using Intent.MetaModel.Service;
using Intent.Modules.AspNet.WebApi;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.EntityFramework.Interop.WebApi.Decorators
{
    public class WebApiControllerDecorator : WebApiControllerDecoratorBase, IHasNugetDependencies
    {
        public WebApiControllerDecorator(IApplicationEventDispatcher applicationEventDispatcher)
        {
        }

        public const string Identifier = "Intent.EntityFramework.Interop.WebApi.ControllerDecorator";

        public override IEnumerable<string> DeclareUsings(IServiceModel mode) => new List<string>()
        {
            "using Intent.Framework;",
            "using Intent.Framework.Core;",
            "using Intent.Framework.EntityFramework;",
        };

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IContextBackingStore _contextBackingStore;";

        public override string ConstructorParams(IServiceModel service) => @"
            , IDbContextFactory dbContextFactory
            , IContextBackingStore contextBackingStore";

        public override string ConstructorInit(IServiceModel service) => @"
            _dbContextFactory = dbContextFactory;
            _contextBackingStore = contextBackingStore;";

        public override string BeforeTransaction(IServiceModel service, IOperationModel operation) => $@"
                using (var dbScope = new DbContextScope(_dbContextFactory, _contextBackingStore, readOnly: { operation.Stereotypes.Any(x => x.Name == "ReadOnly").ToString().ToLower() }))
                {{";

        public override string AfterCallToAppLayer(IServiceModel service, IOperationModel operation) => operation.Stereotypes.All(x => x.Name != "ReadOnly") ? @"
                    dbScope.SaveChanges();" : "";

        public override string AfterTransaction(IServiceModel service, IOperationModel operation) => @"
                }";

        public override int Priority { get; set; } = -200;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkEntityFramework,
            };
        }
    }
}