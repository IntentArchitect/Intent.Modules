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
    partial class ProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<IFileTemplate>, IHasTemplateDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        public ProjectItemTemplatePartialTemplate(string templateId, IProject project, IFileTemplate model) : base(templateId, project, model)
        {
            AddNugetDependency(NugetPackages.IntentModulesCommon);
            AddNugetDependency(NugetPackages.IntentRoslynWeaverAttributes);
            if (!string.IsNullOrWhiteSpace(GetModeler()?.NuGetDependency))
            {
                AddNugetDependency(new NugetPackageInfo(GetModeler().NuGetDependency, GetModeler().NuGetVersion));
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
                { "Module Dependency", GetModeler()?.ModuleDependency },
                { "Module Dependency Version", GetModeler()?.ModuleVersion }, 
                { "ModelId", Model.Id }
            });
        }

        private IModelerReference GetModeler()
        {
            return Model.GetFileTemplateSettings().Modeler() != null ? new ModelerReference(Model.GetFileTemplateSettings().Modeler()) : null;
        }

        private string GetModelType()
        {
            var modelType = Model.GetFileTemplateSettings().ModelType() != null ? new ModelerModelType(Model.GetFileTemplateSettings().ModelType()) : null;
            if (Model.GetFileTemplateSettings().CreationMode().IsFileperModel())
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
