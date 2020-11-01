using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial
{
    partial class ProjectItemTemplatePartialTemplate : CSharpTemplateBase<FileTemplateModel>, IHasTemplateDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        public ProjectItemTemplatePartialTemplate(string templateId, IProject project, FileTemplateModel model) : base(templateId, project, model)
        {
            AddNugetDependency(NugetPackages.IntentModulesCommon);
        }

        public IList<string> OutputFolder => Model.GetFolderPath().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                relativeLocation: $"{FolderPath}",
                fileName: $"{Model.Name}Partial"
                );
        }

        public string GetTemplateId()
        {
            return $"{Project.ApplicationName()}.{FolderNamespace}";
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(
                modelId: Model.Id,
                templateId: GetTemplateId(),
                templateType: "File Template",
                role: GetRole()));

            if (Model.GetModelType() != null)
            {
                Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private string GetRole()
        {
            return Model.GetRole() ?? GetTemplateId();
        }

        private string GetModelType()
        {
            return Model.GetModelName();
        }

        private string GetTemplateBaseClass()
        {
            return nameof(IntentTemplateBase);
        }
    }
}
