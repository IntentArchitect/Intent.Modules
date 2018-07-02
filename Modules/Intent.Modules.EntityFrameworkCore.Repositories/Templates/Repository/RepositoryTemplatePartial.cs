using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
{
    partial class RepositoryTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, IPostTemplateCreation, IBeforeTemplateExecutionHook
    {
        public const string Identifier = "Intent.EntityFrameworkCore.Repositories.Implementation";
        private ITemplateDependancy _entityStateTemplateDependancy;
        private ITemplateDependancy _entityInterfaceTemplateDependancy;
        private ITemplateDependancy _repositoryInterfaceTemplateDependancy;
        private ITemplateDependancy _dbContextTemplateDependancy;

        public RepositoryTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            _entityStateTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Interface Template Id"], (to) => to.Id == Model.Id);
            _repositoryInterfaceTemplateDependancy = TemplateDependancy.OnModel(RepositoryInterfaceTemplate.Identifier, Model);
            _dbContextTemplateDependancy = TemplateDependancy.OnTemplate(DbContextTemplate.Identifier);
        }

        public string EntityInterfaceName
        {
            get
            {
                var template = Project.FindTemplateInstance<IHasClassDetails>(_entityInterfaceTemplateDependancy);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string EntityName
        {
            get
            {
                var template = Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependancy);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string RepositoryContractName => Project.FindTemplateInstance<IHasClassDetails>(_repositoryInterfaceTemplateDependancy)?.ClassName ?? $"I{ClassName}";

        public string DbContextName => Project.FindTemplateInstance<IHasClassDetails>(_dbContextTemplateDependancy)?.ClassName ?? $"{Model.Application.Name}DbContext";

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

        public void PreProcess()
        {

        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                _entityStateTemplateDependancy,
                _entityInterfaceTemplateDependancy,
                _repositoryInterfaceTemplateDependancy,
                _dbContextTemplateDependancy,
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
                {
                    new NugetPackageInfo("Intent.Framework.EntityFrameworkCore", "1.0.0", null),
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }

        public void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IHasClassDetails>(_repositoryInterfaceTemplateDependancy);
            if (contractTemplate == null)
            {
                return;
            }

            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", $"{contractTemplate.Namespace}.{contractTemplate.ClassName}"},
                { "ConcreteType", $"{Namespace}.{ClassName}" },
                { "InterfaceTypeTemplateId", _repositoryInterfaceTemplateDependancy.TemplateIdOrName },
                { "ConcreteTypeTemplateId", Identifier }
            });
        }
    }
}
