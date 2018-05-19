using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.Entities.DDD.Templates.EntitySpecification
{
    partial class EntitySpecificationTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate
    {
        public const string Identifier = "Intent.Entities.DDD.EntitySpecification";

        public EntitySpecificationTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel(DomainEntityStateTemplate.Identifier, Model))?.ClassName 
            ?? Model.Name;

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
