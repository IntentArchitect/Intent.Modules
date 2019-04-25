using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    partial class RepositoryInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, IPostTemplateCreation
    {
        public const string Identifier = "Intent.Entities.Repositories.Api.Interface";
        private ITemplateDependency _entityStateTemplateDependancy;
        private ITemplateDependency _entityInterfaceTemplateDependancy;

        public RepositoryInterfaceTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            _entityStateTemplateDependancy = TemplateDependency.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependancy = TemplateDependency.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Interface Template Id"], (to) => to.Id == Model.Id);
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependancy)?.ClassName ?? Model.Name;

        public string EntityInterfaceName => Project.FindTemplateInstance<IHasClassDetails>(_entityInterfaceTemplateDependancy)?.ClassName ?? $"I{Model.Name}";

        public string PrimaryKeyType => Types.Get(Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Type) ?? "Guid";

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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                _entityInterfaceTemplateDependancy,
                _entityStateTemplateDependancy
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
                {
                    new NugetPackageInfo("Intent.Framework.Domain", "1.0.0", null),
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }
    }
}
