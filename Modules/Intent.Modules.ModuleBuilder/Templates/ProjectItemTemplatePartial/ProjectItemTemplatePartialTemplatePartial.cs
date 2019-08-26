using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using static Intent.Modules.ModuleBuilder.Helpers.TemplateHelper;

namespace Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial
{
    partial class ProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        private readonly IEnumerable<IClass> _templateModels;

        public ProjectItemTemplatePartialTemplate(string templateId, IProject project, IClass model, IEnumerable<IClass> templateModels) : base(templateId, project, model)
        {
            _templateModels = templateModels;
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);
        public string FolderNamespace => string.Join(".", FolderBaseList);

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"${{Model.Name}}Partial",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new INugetPackageInfo[]
                {
                    NugetPackages.IntentModulesCommon,
                    NugetPackages.IntentRoslynWeaverAttributes
                }
                .Union(base.GetNugetDependencies())
                .ToArray();
        }

        public string GetTemplateId()
        {
            return $"{Project.ApplicationName()}.{Model.Name}";
        }

        private string GetModelType()
        {
            var type = Model.GetTargetModel();
            if (Model.GetCreationMode() == CreationMode.SingleFileListModel)
            {
                type = $"IList<{type}>";
            }

            return type;
        }

        private bool HasDeclaresUsings()
        {
            return Model.GetStereotypeProperty<bool>("Template Settings", "Declare Usings");
        }

        private bool HasTemplateDependencies()
        {
            return Model.HasStereotype("Template Dependency");
        }

        private bool HasDecorators()
        {
            return !string.IsNullOrEmpty(Model.GetExposedDecoratorContractType());
        }

        private IReadOnlyCollection<TemplateDependencyInfo> GetTemplateDependencies()
        {
            return TemplateHelper.GetTemplateDependencies(this, Model, _templateModels);
        }

        private string GetConfiguredInterfaces()
        {
            var interfaceList = new List<string>();

            if (HasDeclaresUsings())
            {
                interfaceList.Add("IDeclareUsings");
            }

            if (HasTemplateDependencies())
            {
                interfaceList.Add("IHasTemplateDependencies");
            }

            if (!string.IsNullOrEmpty(Model.GetExposedDecoratorContractType()))
            {
                interfaceList.Add($"IHasDecorators<{Model.GetExposedDecoratorContractType()}>");
            }

            return interfaceList.Any() ? (", " + string.Join(", ", interfaceList)) : string.Empty;
        }
    }
}
