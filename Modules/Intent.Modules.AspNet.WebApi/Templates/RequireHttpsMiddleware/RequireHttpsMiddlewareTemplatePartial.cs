using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.AspNet.WebApi.Templates.RequireHttpsMiddleware
{
    public partial class RequireHttpsMiddlewareTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.AspNet.WebApi.RequireHttpsMiddleware";
        public RequireHttpsMiddlewareTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "RequireHttpsMiddleware",
                fileExtension: "cs",
                defaultLocationInProject: @"App_Start"

                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftOwin,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
