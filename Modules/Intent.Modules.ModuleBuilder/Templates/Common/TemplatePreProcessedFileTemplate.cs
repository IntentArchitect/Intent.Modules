using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using Mono.TextTemplating;

namespace Intent.Modules.ModuleBuilder.Templates.Common
{
    public class TemplatePreProcessedFileTemplate : IntentProjectItemTemplateBase<IClass>
    {
        private readonly string _t4TemplateId;
        private readonly string _partialTemplateId;

        public TemplatePreProcessedFileTemplate(
            string templateId,
            IProject project,
            IClass model,
            string t4TemplateId,
            string partialTemplateId)
                : base(templateId, project, model)
        {
            _t4TemplateId = t4TemplateId;
            _partialTemplateId = partialTemplateId;
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            var metadata = new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Templates/${Model.Name}");

            metadata.CustomMetaData.Add("Depends On", "${Model.Name}.tt");

            return metadata;
        }

        public override string TransformText()
        {
            var t4TemplateInstance = Project.FindTemplateInstance(_t4TemplateId, Model);
            var partialTemplateInstance = Project.FindTemplateInstance(_partialTemplateId, Model);
            var partialTemplateMetadata = partialTemplateInstance.GetMetaData();
            var templateGenerator = new TemplateGenerator();

            templateGenerator.PreprocessTemplate(
                inputFileName: string.Empty,
                className: partialTemplateMetadata.CustomMetaData["ClassName"],
                classNamespace: partialTemplateMetadata.CustomMetaData["Namespace"],
                inputContent: t4TemplateInstance.RunTemplate(),
                language: out _,
                references: out _,
                outputContent: out var outputContent);

            return outputContent;
        }
    }
}
