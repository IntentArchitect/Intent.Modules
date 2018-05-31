using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.Repository
{
    partial class RepositoryTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.EntityFramework.RepositoryImplementation";
        private ITemplateDependancy _entityStateTemplateDependancy;
        private ITemplateDependancy _entityInterfaceTemplateDependancy;

        public RepositoryTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            _entityStateTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Interface Template Id"], (to) => to.Id == Model.Id);
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                _entityStateTemplateDependancy,
                _entityInterfaceTemplateDependancy
            };
        }

        public string EntityInterfaceName => Project.FindTemplateInstance<IHasClassDetails>(_entityInterfaceTemplateDependancy)?.ClassName ?? $"I{Model.Name}";
        public string EntityName => Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependancy)?.ClassName ?? $"{Model.Name}";
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
