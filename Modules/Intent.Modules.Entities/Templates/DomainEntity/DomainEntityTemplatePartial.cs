using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    partial class DomainEntityTemplate : IntentRoslynProjectItemTemplateBase<ClassModel>, ITemplate, IHasDecorators<DomainEntityDecoratorBase>, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.Entities.DomainEntity";
        private readonly IList<DomainEntityDecoratorBase> _decorators = new List<DomainEntityDecoratorBase>();

        public DomainEntityTemplate(ClassModel model, IProject project)
            : base(Identifier, project, model)
        {
            
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, new TemplateVersion(1, 0)));
        }

        public override void OnCreated()
        {
            //Types.AddClassTypeSource(ClassTypeSource.InProject(Project, DomainEntityTemplate.Identifier, nameof(ICollection)));
            Types.AddClassTypeSource(CSharpTypeSource.InProject(Project, DomainEntityInterfaceTemplate.Identifier));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Model.Name}",
                @namespace: "${Project.ProjectName}"
                );
        }

        public void AddDecorator(DomainEntityDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string Constructors(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

        public string GetParametersDefinition(IOperation operation)
        {
            return operation.Parameters.Any() 
                ? operation.Parameters.Select(x => this.ConvertType(x.Type) + " " + x.Name.ToCamelCase()).Aggregate((x, y) => x + ", " + y) 
                : "";
        }

        public string EmitOperationReturnType(IOperation o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{this.ConvertType(o.ReturnType.Type)}>" : this.ConvertType(o.ReturnType.Type);
        }
    }
}
