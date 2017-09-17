using Intent.MetaModel.Dto.Old;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Common;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;

namespace Intent.Modules.Application.Contracts.Legacy.DTO
{
    public class DTOTemplate : IntentRoslynProjectItemTemplateBase<DtoModel>, ITemplate, IHasAssemblyDependencies
    {
        public const string Identifier = "Intent.Application.Contracts.DTO.Legacy";

        public DTOTemplate(IProject project, DtoModel model, string identifier = Identifier)
            : base (identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                    overwriteBehaviour: OverwriteBehaviour.Always,
                    fileName: Model.Class.ClassType.TypeName,
                    fileExtension: "cs",
                    defaultLocationInProject: "Generated\\DTOs",
                    className : Model.Class.ClassType.TypeName,
                    @namespace: "${Project.ProjectName}"
                );
        }

        public override string TransformText()
        {
            Model.Class.ClassType = TypeModel.Class(this.Namespace, this.ClassName); // Set Namespace...
            var genericClassTemplate = new GenericClassTemplate(this.Namespace, Model.Class);
            return genericClassTemplate.RunTemplate();
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new IAssemblyReference[]
            {
                new GacAssemblyReference("System.Runtime.Serialization"),
            };
        }
    }
}
