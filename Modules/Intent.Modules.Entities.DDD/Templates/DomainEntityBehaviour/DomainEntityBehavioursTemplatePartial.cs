using System.Collections;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Engine;
using Intent.Templates

namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    partial class DomainEntityBehavioursTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IPostTemplateCreation
    {
        public const string Identifier = "Intent.Entities.DDD.Behaviours";

        public DomainEntityBehavioursTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            Types.AddClassTypeSource(ClassTypeSource.InProject(Project, DomainEntityInterfaceTemplate.Identifier, nameof(IEnumerable)));
        }

        public string ClassStateName => Model.Name;

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "I${Model.Name}Behaviours",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "I${Model.Name}Behaviours",
                @namespace: "${Project.ProjectName}"
                );
        }

        private string GetParametersDefinition(IOperation operation)
        {
            return operation.Parameters.Any()
                ? operation.Parameters.Select(x => this.ConvertType(x.Type) + " " + x.Name.ToCamelCase()).Aggregate((x, y) => x + ", " + y)
                : "";
        }

        public string EmitOperationReturnType(IOperation o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{this.ConvertType(o.ReturnType.Type)}>" : this.ConvertType(o.ReturnType.Type);
        }
    }
}
