using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFramework.Repositories.Templates.EntityCompositionVisitor;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.Repository
{
    partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasTemplateDependencies, ITemplatePostCreationHook, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.Implementation";
        private ITemplateDependency _entityStateTemplateDependency;
        private ITemplateDependency _entityInterfaceTemplateDependency;
        private ITemplateDependency _repositoryInterfaceTemplateDependency;
        private ITemplateDependency _dbContextTemplateDependency;
        private ITemplateDependency _deleteVisitorTemplateDependency;

        public RepositoryTemplate(ClassModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            _entityStateTemplateDependency = TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependency = TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Interface Template Id"], (to) => to.Id == Model.Id);
            _repositoryInterfaceTemplateDependency = TemplateDependency.OnModel(EntityRepositoryInterfaceTemplate.Identifier, Model);
            _dbContextTemplateDependency = TemplateDependency.OnTemplate(DbContextTemplate.Identifier);
            _deleteVisitorTemplateDependency = TemplateDependency.OnTemplate(EntityCompositionVisitorTemplate.Identifier);
        }

        public string EntityCompositionVisitorName
        {
            get
            {
                var template = Project.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(EntityCompositionVisitorTemplate.Identifier));
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : null;
            }
        }

        public string EntityInterfaceName
        {
            get
            {
                var template = Project.FindTemplateInstance<IClassProvider>(_entityInterfaceTemplateDependency);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string EntityName
        {
            get
            {
                var template = Project.FindTemplateInstance<IClassProvider>(_entityStateTemplateDependency);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string RepositoryContractName => Project.FindTemplateInstance<IClassProvider>(_repositoryInterfaceTemplateDependency)?.ClassName ?? $"I{ClassName}";

        public string DbContextName => Project.FindTemplateInstance<IClassProvider>(_dbContextTemplateDependency)?.ClassName ?? $"{Model.Application.Name}DbContext";

        public string DeleteVisitorName => Project.FindTemplateInstance<IClassProvider>(_deleteVisitorTemplateDependency)?.ClassName ?? $"{Model.Application.Name}DeleteVisitor";

        public string PrimaryKeyType => Types.Get(Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Type)?.Name ?? "Guid";

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Name.ToPascalCase() ?? "Id";

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Repository",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
        
        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            // GCB - Looks like an old way of doing things (comment as at V3.0):
            return base.GetTemplateDependencies().Union(new[]
            {
                _entityStateTemplateDependency,
                _entityInterfaceTemplateDependency,
                _repositoryInterfaceTemplateDependency,
                _dbContextTemplateDependency,
                _deleteVisitorTemplateDependency,
            });
        }

        public override void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(_repositoryInterfaceTemplateDependency);
            if (contractTemplate == null)
            {
                return;
            }

            Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            {
                { "InterfaceType", $"{contractTemplate.Namespace}.{contractTemplate.ClassName}"},
                { "ConcreteType", $"{Namespace}.{ClassName}" },
                { "InterfaceTypeTemplateId", _repositoryInterfaceTemplateDependency.TemplateId },
                { "ConcreteTypeTemplateId", Identifier }
            });
        }
    }
}
