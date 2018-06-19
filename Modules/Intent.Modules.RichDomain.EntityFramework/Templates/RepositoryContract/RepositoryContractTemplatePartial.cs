using System.Collections.Generic;
using Intent.Modules.RichDomain.Templates.EntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.RichDomain.EntityFramework.Templates.RepositoryContract
{
    partial class RepositoryContractTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.RichDomain.EntityFramework.RepositoryContract";

        public RepositoryContractTemplate(Class model, IProject project)
            : base (Identifier, project, model)
        {
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnModel(DomainEntityStateTemplate.Identifier, Model)
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }


        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"I{Model.Name}Repository",
                fileExtension: "cs",
                defaultLocationInProject: "RepositoryContract"
                );
        }
    }
}
