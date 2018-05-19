using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Entities.DDD.Templates.RepositoryInterface
{
    partial class RepositoryInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate
    {
        public const string Identifier = "Intent.Entities.DDD.RepositoryInterface";

        public RepositoryInterfaceTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel(DomainEntityStateTemplate.Identifier, Model))?.ClassName
                                         ?? Model.Name;

        public string EntityInterfaceName => Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel(DomainEntityInterfaceTemplate.Identifier, Model))?.ClassName
                                         ?? $"I{Model.Name}";

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

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                new NugetPackageInfo("Intent.Framework.Domain", "1.0.0-pre2", null),
            };
        }
    }
}
