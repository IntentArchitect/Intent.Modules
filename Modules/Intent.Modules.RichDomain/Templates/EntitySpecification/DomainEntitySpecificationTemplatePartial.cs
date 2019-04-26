using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates;

namespace Intent.Modules.RichDomain.Templates.EntitySpecification
{
    partial class DomainEntitySpecificationTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate
    {
        public const string Identifier = "Intent.RichDomain.EntitySpecification";

        public DomainEntitySpecificationTemplate(Class model, IProject project)
            : base (Identifier, project, model)
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
                        fileName: $"{Model.Name}Specification",
                        fileExtension: "cs",
                        defaultLocationInProject: "Specification"
                );
        }
    }
}
