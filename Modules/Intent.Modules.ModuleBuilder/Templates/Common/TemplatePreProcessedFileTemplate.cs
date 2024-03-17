using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Templates;
using Mono.TextTemplating;

namespace Intent.Modules.ModuleBuilder.Templates.Common
{
    public class TemplatePreProcessedFileTemplate : IntentFileTemplateBase<TemplateRegistrationModel>
    {
        private readonly string _t4TemplateId;
        private readonly string _partialTemplateId;

        public TemplatePreProcessedFileTemplate(
            string templateId,
            IOutputTarget project,
            TemplateRegistrationModel model,
            string t4TemplateId,
            string partialTemplateId)
                : base(templateId, project, model)
        {
            _t4TemplateId = t4TemplateId;
            _partialTemplateId = partialTemplateId;
        }

        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name.ToCSharpIdentifier().RemoveSuffix("Template") }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var metadata = new TemplateFileConfig(
                fileName: $"{TemplateName}",
                fileExtension: "cs",
                relativeLocation: $"{FolderPath}",
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic);

            metadata.CustomMetadata.Add("Depends On", $"{TemplateName}.tt");

            return metadata;
        }

        public override string TransformText()
        {
            var t4TemplateInstance = (IntentTemplateBase)Project.FindTemplateInstance(_t4TemplateId, Model);
            var partialTemplateInstance = Project.FindTemplateInstance(_partialTemplateId, Model);
            var partialTemplateMetadata = partialTemplateInstance.GetMetadata();
            var templateGenerator = new TemplateGenerator();

            var generatedT4Content = t4TemplateInstance.RunTemplate();
            var t4TemplateIsDifferent = !t4TemplateInstance.TryGetExistingFileContent(out var existingT4Content) ||
                                        generatedT4Content.Trim() != existingT4Content.Trim();

            // The output of pre-processing below is slightly different to how it happens when done in VS itself, so we don't
            // want to re-run the pre-processing unless something is different.
            if (TryGetExistingFileContent(out var existingFileContent) &&
                !t4TemplateIsDifferent &&
                !PathHasChanged())
            {
                return existingFileContent;
            }

            var hasErrors = !templateGenerator.PreprocessTemplate(
                inputFileName: string.Empty,
                className: partialTemplateMetadata.CustomMetadata["ClassName"],
                classNamespace: partialTemplateMetadata.CustomMetadata["Namespace"],
                inputContent: generatedT4Content,
                language: out _,
                references: out _,
                outputContent: out var outputContent);

            if (hasErrors)
            {
                throw new Exception($"An error was found while generating the .cs code-behind file for template [{t4TemplateInstance}]: {string.Join(";", templateGenerator.Errors.Cast<CompilerError>().Select(x => x.ErrorText))}");
            }

            return outputContent;
        }

        private bool PathHasChanged()
        {
            var fileLog = ExecutionContext.GetPreviousExecutionLog()?.TryGetFileLog(this);
            if (fileLog == null)
            {
                return true;
            }

            var result = Path.GetFullPath(fileLog.FilePath) != Path.GetFullPath(FileMetadata.GetFilePath());

            return result;
        }
    }
}
