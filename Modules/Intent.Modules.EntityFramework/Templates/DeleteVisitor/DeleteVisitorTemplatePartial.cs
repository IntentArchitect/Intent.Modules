using Intent.Modules.RichDomain.Templates.EntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.EntityFramework.Templates.DeleteVisitor
{
    partial class DeleteVisitorTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<Class>>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.EntityFramework.DeleteVisitor";

        public DeleteVisitorTemplate(IEnumerable<Class> models, IProject project)
            : base (Identifier, project, models)
        {

        }

        public string BoundedContextName => Project.ApplicationName();

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return Model.Select(x => TemplateDependancy.OnModel(DomainEntityStateTemplate.Identifier, x)).ToArray();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{BoundedContextName}DeleteVisitor",
                fileExtension: "cs",
                defaultLocationInProject: "Generated\\DeleteVisitor"

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
    }
}
