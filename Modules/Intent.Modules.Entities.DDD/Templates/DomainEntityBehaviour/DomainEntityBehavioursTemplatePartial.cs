using System.Collections;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Templates;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.Entities.DDD.Templates.DomainEntityBehaviour
{
    partial class DomainEntityBehavioursTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.Entities.DDD.Behaviours";

        public DomainEntityBehavioursTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            Types.AddClassTypeSource(CSharpTypeSource.InProject(Project, DomainEntityInterfaceTemplate.Identifier));
        }

        public string ClassStateName => Model.Name;

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
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
