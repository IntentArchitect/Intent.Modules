using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;
using JetBrains.Annotations;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public abstract class VisualStudioProjectTemplateBase : IntentFileTemplateBase<IVisualStudioProject>, ITemplate, IVisualStudioProjectTemplate
    {

        protected VisualStudioProjectTemplateBase([NotNull]string templateId, [NotNull]IOutputTarget project, [NotNull]IVisualStudioProject model) : base(templateId, project, model)
        {
        }

        public string ProjectId => Model.Id;
        public string Name => Model.Name;
        public string FilePath => GetMetadata().GetFullLocationPathWithFileName();

        public string LoadContent()
        {
            var change = ExecutionContext.ChangeManager.FindChange(FilePath);

            return change != null
                ? change.Content
                : File.ReadAllText(Path.GetFullPath(FilePath));
        }

        //public void ChangeOutput(string content)
        //{
        //    var change = ExecutionContext.ChangeManager.FindChange(FilePath);

        //    // Normalize the content of both by parsing with no whitespace and calling .ToString()
        //    var targetContent = XDocument.Parse(content).ToString();
        //    var existingContent = change != null
        //        ? XDocument.Parse(change.Content).ToString()
        //        : XDocument.Load(FilePath).ToString();

        //    if (existingContent == targetContent)
        //    {
        //        return;
        //    }

        //    if (change != null)
        //    {
        //        change.ChangeContent(content);
        //        return;
        //    }

        //    _sfEventDispatcher.Publish(new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
        //    {
        //        { "FullFileName", filePath },
        //        { "Content", content },
        //    }));
        //}

        public IEnumerable<INugetPackageInfo> RequestedNugetPackages()
        {
            return OutputTarget.NugetPackages();
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Project.Application.EventDispatcher.Publish(new VisualStudioProjectCreatedEvent(Project.Id, this));
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                codeGenType: CodeGenType.Basic,
                fileName: Project.Name,
                fileExtension: "csproj",
                relativeLocation: ""
            );
        }

        public override string RunTemplate()
        {
            if (GetTemplateFileConfig().OverwriteBehaviour != OverwriteBehaviour.OnceOff)
            {
                // Unless onceOff, then on subsequent SF runs, the SF shows two outputs for the same .csproj file.
                throw new Exception("Template must be configured with OverwriteBehaviour.OnceOff.");
            }

            return TransformText();
        }

    }
}