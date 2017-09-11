using System.Collections.Generic;
using System.Linq;
using Intent.Packages.IdentityServer.Templates.AspNetIdentityModel;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.IdentityServer.Templates.AspNetIdentityUserService
{
    public partial class AspNetIdentityUserServiceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasNugetDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.IdentityServer.AspNetIdentity.UserService";

        public AspNetIdentityUserServiceTemplate(IProject project)
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
                fileName: "UserService",
                fileExtension: "cs",
                defaultLocationInProject: "Services");
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IdentityServer3AspNetIdentity,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(AspNetIdentityModelTemplate.Identifier)
            };
        }
    }
}
