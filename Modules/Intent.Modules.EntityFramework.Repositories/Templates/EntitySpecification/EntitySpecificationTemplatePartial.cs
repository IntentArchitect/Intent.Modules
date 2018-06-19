using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.EntityFramework.Repositories.Templates.EntitySpecification
{
    partial class EntitySpecificationTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IPostTemplateCreation
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.EntitySpecification";
        private ITemplateDependancy _entityStateTemplateDependancy;

        public EntitySpecificationTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            _entityStateTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependancy)?.ClassName ?? Model.Name;

        public string BaseClass => $"DomainSpecificationBase<{EntityStateName}, {ClassName}>";

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Specification",
                fileExtension: "cs",
                defaultLocationInProject: "Specifications",
                className: "${Model.Name}Specification",
                @namespace: "${Project.ProjectName}"
                );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                new NugetPackageInfo("Intent.Framework.Domain", "0.1.11-beta", null),
            };
        }
    }
}
