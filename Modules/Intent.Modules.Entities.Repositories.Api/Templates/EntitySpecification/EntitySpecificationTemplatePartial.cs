using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.EntitySpecification
{
    partial class EntitySpecificationTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IPostTemplateCreation
    {
        public const string Identifier = "Intent.Entities.Repositories.Api.EntitySpecification";
        private ITemplateDependency _entityStateTemplateDependancy;

        public EntitySpecificationTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            _entityStateTemplateDependancy = TemplateDependency.OnModel<IClass>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == Model.Id);
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependancy)?.ClassName ?? Model.Name;

        public string BaseClass => $"DomainSpecificationBase<{EntityStateName}, {ClassName}>";

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
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
                new NugetPackageInfo("Intent.Framework.Domain", "1.0.0", null),
            };
        }
    }
}
