using Intent.Modules.AspNet.WebApi;
using Intent.Modules.AspNet.WebApi.Legacy;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.EntityFramework.Interop.WebApi.Legacy
{
    public class EntityFrameworkDistributionDecorator : BaseDistributionDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.EntityFramework.Interop.WebApi.Legacy";

        public EntityFrameworkDistributionDecorator()
        {
        }

        public override IEnumerable<string> DeclareUsings(ServiceModel mode) => new List<string>()
        {
            "using Intent.Framework;",
            "using Intent.Framework.Core.Context;",
            "using Intent.Framework.EntityFramework;",
        };

        public override string DeclarePrivateVariables(ServiceModel service) => @"
        private readonly IDbContextFactory _dbContextFactory;";

        public override string ConstructorParams(ServiceModel service) => @"
            , IDbContextFactory dbContextFactory";

        public override string ConstructorInit(ServiceModel service) => @"
            _dbContextFactory = dbContextFactory;";

        public override string BeforeTransaction(ServiceModel service, ServiceOperationModel operation) => $@"
                using (var dbScope = new DbContextScope(_dbContextFactory, readOnly: { operation.IsReadOnly.ToString().ToLower() }))
                {{";

        public override string AfterCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => !operation.IsReadOnly ? @"
                    dbScope.SaveChanges();" : "";

        public override string AfterTransaction(ServiceModel service, ServiceOperationModel operation) => @"
                }";

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkEntityFramework,
            };
        }

        public override int Priority { get; set; } = -200;
    }
}