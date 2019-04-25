using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Eventing;
using Intent.Templates
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.IdentityServer.Templates.AspNetIdentityModel
{
    // TODO: Break this template out into the various entities in the Identity Model
    public partial class AspNetIdentityModelTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.IdentityServer.AspNetIdentity.Model";

        private readonly IApplicationEventDispatcher _eventDispatcher;
        public const string DB_CONTEXT_NAME = "IdentityDbContext";

        public AspNetIdentityModelTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(Identifier, project, null)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "AspNetIdentityModel",
                fileExtension: "cs",
                defaultLocationInProject: "Models");
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftAspNetIdentityCore,
                NugetPackages.MicrosoftAspNetIdentityEntityFramework,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
            {
                { "Name", $"IdentityDB" },
                { "ConnectionString", $"Server=.;Initial Catalog={ Project.Application.SolutionName }Identity;Integrated Security=true;MultipleActiveResultSets=True" },
                { "ProviderName", "System.Data.SqlClient" },
            });
        }
    }
}
