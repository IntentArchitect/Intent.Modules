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
    partial class ProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<FileTemplateModel>, IHasTemplateDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        public ProjectItemTemplatePartialTemplate(string templateId, IProject project, FileTemplateModel model) : base(templateId, project, model)
        {
            AddNugetDependency(NugetPackages.IntentModulesCommon);
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath().Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);
        public string FolderNamespace => string.Join(".", FolderBaseList);

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"${{Model.Name}}Partial",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
            );
        }

        public string GetTemplateId()
        {
            return $"{Project.ApplicationName()}.{FolderNamespace}.{Model.Name}";
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
