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
                { "TemplateType", "File Template" }
            });
        }

        private IModelerReference GetModeler()
        {
            return Model.Modeler() != null ? new ModelerReference(Model.Modeler()) : null;
        }

        private string GetModelType()
        {
            var modelType = Model.ModelType() != null ? new ModelerModelType(Model.ModelType()) : null;
            if (Model.CreationMode() == FileTemplateExtensions.CreationModeOptions.FileperModel)
            {
                return modelType?.InterfaceName ?? "object";
            }

            return modelType == null ? "object" : $"IList<{modelType.InterfaceName}>";
        }

        private string GetTemplateBaseClass()
        {
            return Model.BaseType();
        }


        private bool HasDecorators()
        {
            return !string.IsNullOrEmpty(Model.GetExposedDecoratorContractType());
        }

        private string GetConfiguredInterfaces()
        {
            var interfaceList = new List<string>();

            if (!string.IsNullOrEmpty(Model.GetExposedDecoratorContractType()))
            {
                interfaceList.Add($"IHasDecorators<{Model.GetExposedDecoratorContractType()}>");
            }

            return interfaceList.Any() ? (", " + string.Join(", ", interfaceList)) : string.Empty;
        }
    }
}
