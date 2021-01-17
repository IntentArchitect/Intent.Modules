using System;
using System.CodeDom.Compiler;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using Mono.TextTemplating;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Types.Api;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp.Templates;

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

        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var Metadata = new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: $"{TemplateName}",
                fileExtension: "cs",
                relativeLocation: $"{FolderPath}");

            Metadata.CustomMetadata.Add("Depends On", $"{TemplateName}.tt");

            return Metadata;
        }

        public override string TransformText()
        {
            var t4TemplateInstance = Project.FindTemplateInstance(_t4TemplateId, Model);
            var partialTemplateInstance = Project.FindTemplateInstance(_partialTemplateId, Model);
            var partialTemplateMetadata = partialTemplateInstance.GetMetadata();
            var templateGenerator = new TemplateGenerator();

            var hasErrors = !templateGenerator.PreprocessTemplate(
                inputFileName: string.Empty,
                className: partialTemplateMetadata.CustomMetadata["ClassName"],
                classNamespace: partialTemplateMetadata.CustomMetadata["Namespace"],
                inputContent: t4TemplateInstance.RunTemplate(),
                language: out _,
                references: out _,
                outputContent: out var outputContent);

            if (hasErrors)
            {
                throw new Exception($"An error was found while generating the .cs code-behind file for template [{t4TemplateInstance}]: {string.Join(";", templateGenerator.Errors.Cast<CompilerError>().Select(x => x.ErrorText))}");
            }

            return outputContent;
        }
    }
}
