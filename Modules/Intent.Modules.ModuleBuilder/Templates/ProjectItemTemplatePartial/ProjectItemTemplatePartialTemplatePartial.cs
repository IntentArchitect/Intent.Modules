using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial
{
    partial class ProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<FileTemplateModel>, IHasTemplateDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        public ProjectItemTemplatePartialTemplate(string templateId, IProject project, FileTemplateModel model) : base(templateId, project, model)
        {
            AddNugetDependency(NugetPackages.IntentModulesCommon);
            AddNugetDependency(NugetPackages.IntentRoslynWeaverAttributes);
            if (!string.IsNullOrWhiteSpace(Model.GetModeler()?.NuGetDependency))
            {
                AddNugetDependency(new NugetPackageInfo(Model.GetModeler().NuGetDependency, Model.GetModeler().NuGetVersion));
            }
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
            Project.Application.EventDispatcher.Publish("TemplateRegistrationRequired", new Dictionary<string, string>()
            {
                { "TemplateId", GetTemplateId() },
                { "TemplateType", "File Template" },
                { "Module Dependency", Model.GetModeler()?.ModuleDependency },
                { "Module Dependency Version", Model.GetModeler()?.ModuleVersion }, 
                { "ModelId", Model.Id }
            });
        }

        private string GetModelType()
        {
            var modelType = Model.GetModelType();
            if (Model.IsFilePerModelTemplateRegistration())
            {
                return modelType?.InterfaceName ?? "object";
            }

            return modelType == null ? "object" : $"IList<{modelType.InterfaceName}>";
        }

        private string GetTemplateBaseClass()
        {
            return nameof(IntentProjectItemTemplateBase);
        }
    }
}
