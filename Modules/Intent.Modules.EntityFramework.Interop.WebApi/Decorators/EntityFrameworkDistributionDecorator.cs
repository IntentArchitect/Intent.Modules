using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.Modules.WebApi;
using Intent.Modules.WebApi.Templates.Controller;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.EntityFramework.Interop.WebApi.Decorators
{
    public class EntityFrameworkDistributionDecorator : DistributionDecoratorBase, IHasNugetDependencies
    {
        public const string Identifier = "Intent.EntityFramework.Interop.WebApi";

        public override IEnumerable<string> DeclareUsings(IServiceModel mode) => new List<string>()
        {
            "using Intent.Framework;",
            "using Intent.Framework.Core.Context;",
            "using Intent.Framework.EntityFramework;",
        };

        public override string DeclarePrivateVariables(IServiceModel service) => @"
        private readonly IDbContextFactory _dbContextFactory;";

        public override string ConstructorParams(IServiceModel service) => @"
            , IDbContextFactory dbContextFactory";

        public override string ConstructorInit(IServiceModel service) => @"
            _dbContextFactory = dbContextFactory;";

        public override string BeforeTransaction(IServiceModel service, IOperationModel operation) => $@"
                using (var dbScope = new DbContextScope(_dbContextFactory, readOnly: { operation.Stereotypes.Any(x => x.Name == "ReadOnly").ToString().ToLower() }))
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