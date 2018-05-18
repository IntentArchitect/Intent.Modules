using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.DDD.Templates.RepositoryInterface
{
    partial class RepositoryInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate//, IHasDecorators<AbstractDomainEntityDecorator>
    {
        public const string Identifier = "Intent.Entities.DDD.RepositoryInterface";

        public RepositoryInterfaceTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public string EntityStateName => Model.Name;
        public string EntityInterfaceName => $"I{Model.Name}";

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "I${Model.Name}Repository",
                fileExtension: "cs",
                defaultLocationInProject: "Repositories",
                className: "I${Model.Name}Repository",
                @namespace: "${Project.ProjectName}"
                );
        }
    }
}
