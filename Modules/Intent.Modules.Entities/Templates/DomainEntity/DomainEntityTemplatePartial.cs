using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    partial class DomainEntityTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate
    {
        public const string Identifier = "Intent.Entities.Entity";

        public DomainEntityTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Model.Name}",
                @namespace: "${Project.ProjectName}"
                );
        }
    }
}
