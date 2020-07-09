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
using Intent.Modules.ModuleBuilder.Api;

namespace Intent.Modules.ModuleBuilder.Templates.Common
{
    public class TemplatePreProcessedFileTemplate : IntentProjectItemTemplateBase<TemplateRegistrationModel>
    {
        private readonly string _t4TemplateId;
        private readonly string _partialTemplateId;

        public TemplatePreProcessedFileTemplate(
            string templateId,
            IProject project,
            TemplateRegistrationModel model,
            string t4TemplateId,
            string partialTemplateId)
                : base(templateId, project, model)
        {
            _t4TemplateId = t4TemplateId;
            _partialTemplateId = partialTemplateId;
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath().Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            var Metadata = new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}");

            Metadata.CustomMetadata.Add("Depends On", "${Model.Name}.tt");

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
