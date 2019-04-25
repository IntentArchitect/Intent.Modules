using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates

namespace Intent.Modules.RichDomain.EntityFramework.Templates.DbContext
{
    partial class DbContextTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<Class>>, ITemplate, IHasNugetDependencies, IRequiresPreProcessing
    {
        public const string Identifier = "Intent.RichDomain.EntityFramework.DbContext";

        private readonly IApplicationEventDispatcher _eventDispatcher;

        public DbContextTemplate(IEnumerable<Class> models, IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, models)
        {
            _eventDispatcher = eventDispatcher;
        }

        public string BoundedContextName => Project.ApplicationName();

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: BoundedContextName + "DbContext",
                fileExtension: "cs",
                defaultLocationInProject: "Generated\\DbContext"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkEntityFramework,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.Config_ConnectionString, new Dictionary<string, string>()
            {
                { "Name", $"{BoundedContextName}DB" },
                { "ConnectionString", $"Server=.;Initial Catalog={ Project.Application.SolutionName };Integrated Security=true;MultipleActiveResultSets=True" },
                { "ProviderName", "System.Data.SqlClient" },
            });
        }
    }
}
