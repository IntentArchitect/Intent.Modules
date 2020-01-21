using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
{
    partial class RepositoryTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFrameworkCore.Repositories.Implementation";
        private ITemplateDependency _entityStateTemplateDependency;
        private ITemplateDependency _entityInterfaceTemplateDependency;
        private ITemplateDependency _repositoryInterfaceTemplateDependency;
        private ITemplateDependency _dbContextTemplateDependency;

        public RepositoryTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
            _entityStateTemplateDependency = TemplateDependency.OnModel<IClass>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependency = TemplateDependency.OnModel<IClass>(GetMetadata().CustomMetadata["Entity Interface Template Id"], (to) => to.Id == Model.Id);
            _repositoryInterfaceTemplateDependency = TemplateDependency.OnModel(EntityRepositoryInterfaceTemplate.Identifier, Model);
            _dbContextTemplateDependency = TemplateDependency.OnTemplate(DbContextTemplate.Identifier);
        }

        public string EntityInterfaceName
        {
            get
            {
                var template = Project.FindTemplateInstance<IHasClassDetails>(_entityInterfaceTemplateDependency);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string EntityName
        {
            get
            {
                var template = Project.FindTemplateInstance<IHasClassDetails>(_entityStateTemplateDependency);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string RepositoryContractName => Project.FindTemplateInstance<IHasClassDetails>(_repositoryInterfaceTemplateDependency)?.ClassName ?? $"I{ClassName}";

        public string DbContextName => Project.FindTemplateInstance<IHasClassDetails>(_dbContextTemplateDependency)?.ClassName ?? $"{Model.Application.Name}DbContext";

        public string PrimaryKeyType => Types.Get(Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Type) ?? "Guid";
        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Name.ToPascalCase() ?? "Id";

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
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

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                _entityStateTemplateDependency,
                _entityInterfaceTemplateDependency,
                _repositoryInterfaceTemplateDependency,
                _dbContextTemplateDependency,
            };
        }

        public void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IHasClassDetails>(_repositoryInterfaceTemplateDependency);
            if (contractTemplate == null)
            {
                return;
            }

            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", $"{contractTemplate.Namespace}.{contractTemplate.ClassName}"},
                { "ConcreteType", $"{Namespace}.{ClassName}" },
                { "InterfaceTypeTemplateId", _repositoryInterfaceTemplateDependency.TemplateIdOrName },
                { "ConcreteTypeTemplateId", Identifier }
            });
        }
    }
}
