using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.RichDomain.EntityFramework.Templates.RepositoryContract;
using Intent.Modules.RichDomain.Templates.EntityState;
using Intent.Modules.RichDomain.Templates.EntityStateInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates

namespace Intent.Modules.RichDomain.EntityFramework.Templates.Repository
{
    partial class RepositoryTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.RichDomain.EntityFramework.Repository";

        public RepositoryTemplate(Class model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnModel(DomainEntityStateTemplate.Identifier, Model),
                TemplateDependancy.OnModel(DomainEntityStateInterfaceTemplate.Identifier, Model),
                TemplateDependancy.OnModel(RepositoryContractTemplate.Identifier, Model)
            };
        }

        public string EntityInterfaceName => Project.FindTemplateInstance<IHasClassDetails>(DomainEntityStateInterfaceTemplate.Identifier, Model).ClassName;
        public string EntityName => Project.FindTemplateInstance<IHasClassDetails>(DomainEntityStateTemplate.Identifier, Model).ClassName;
        public string RepositoryContractName => Project.FindTemplateInstance<IHasClassDetails>(RepositoryContractTemplate.Identifier, Model).ClassName;

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}Repository",
                fileExtension: "cs",
                defaultLocationInProject: "Repository",
                className: "${Model.Name}Repository",
                @namespace: "${Project.Name}"
                );
        }
    }
}
