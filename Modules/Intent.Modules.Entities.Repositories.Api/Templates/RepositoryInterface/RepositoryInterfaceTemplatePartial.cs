using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    partial class RepositoryInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, IPostTemplateCreation
    {
        public const string Identifier = "Intent.Entities.Repositories.Api.Interface";
        private ITemplateDependancy _entityStateTemplateDependancy;
        private ITemplateDependancy _entityInterfaceTemplateDependancy;

        public RepositoryInterfaceTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            _entityStateTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Interface Template Id"], (to) => to.Id == Model.Id);
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependancy)?.ClassName ?? Model.Name;

        public string EntityInterfaceName => Project.FindTemplateInstance<IHasClassDetails>(_entityInterfaceTemplateDependancy)?.ClassName ?? $"I{Model.Name}";

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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
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
