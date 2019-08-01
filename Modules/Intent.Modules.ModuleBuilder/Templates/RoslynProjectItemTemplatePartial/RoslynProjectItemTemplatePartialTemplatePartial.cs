using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Modules.ModuleBuilder.Templates.ProjectItemTemplatePartial;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.RoslynProjectItemTemplatePartial
{
    partial class RoslynProjectItemTemplatePartialTemplate : IntentRoslynProjectItemTemplateBase<IClass>
    {
        public const string TemplateId = "Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial";
        private readonly IEnumerable<IClass> _templateModels;

        public RoslynProjectItemTemplatePartialTemplate(string templateId, IProject project, IClass model, IEnumerable<IClass> templateModels) : base(templateId, project, model)
        {
            _templateModels = templateModels;
        }

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
                defaultLocationInProject: "Templates\\${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.Templates.${Model.Name}"
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

        private bool HasTemplateDependancies()
        {
            return Model.HasStereotype("Template Dependancy");
        }

        private IReadOnlyCollection<string> GetTemplateDependantIds()
        {
            var ids = Model.Stereotypes
                .Where(p => p.Name == "Template Dependancy")
                .Select(s => s.Properties.FirstOrDefault(p => p.Key == "Template Name")?.Value)
                .Where(p => !string.IsNullOrEmpty(p))
                .SelectMany(s => _templateModels.Where(p => p.Name == s))
                .Select(s => 
                {
                    if (s.IsCSharpTemplate())
                    {
                        var templateInstance = this.Project.FindTemplateInstance<RoslynProjectItemTemplatePartialTemplate>(RoslynProjectItemTemplatePartialTemplate.TemplateId, s);
                        return templateInstance.GetTemplateId();
                    }
                    else if (s.IsFileTemplate())
                    {
                        var templateInstance = this.Project.FindTemplateInstance<ProjectItemTemplatePartialTemplate>(ProjectItemTemplatePartialTemplate.TemplateId, s);
                        return templateInstance.GetTemplateId();
                    }
                    return null;
                })
                .Where(p => p != null)
                .ToArray();
            return ids;
        }

        private string GetConfiguredInterfaces()
        {
            var interfaceList = new List<string>();

            if (HasDeclaresUsings())
            {
                interfaceList.Add("IDeclareUsings");
            }

            if (HasTemplateDependancies())
            {
                interfaceList.Add("IHasTemplateDependencies");
            }

            return interfaceList.Any() ? (", " + string.Join(", ", interfaceList)) : string.Empty;
        }
    }
}
