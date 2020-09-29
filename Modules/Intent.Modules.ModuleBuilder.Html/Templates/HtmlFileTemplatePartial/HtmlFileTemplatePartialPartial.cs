using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Html.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Html.Templates.HtmlFileTemplatePartial
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class HtmlFileTemplatePartial : IntentRoslynProjectItemTemplateBase<HtmlFileTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Html.Templates.HtmlFileTemplatePartial";

        public HtmlFileTemplatePartial(IProject project, HtmlFileTemplateModel model) : base(TemplateId, project, model)
        {
            AddNugetDependency(NugetPackages.IntentCommonHtml);
            //if (!string.IsNullOrWhiteSpace(Model.GetModule().NuGetPackageId))
            //{
            //    AddNugetDependency(new NugetPackageInfo(Model.GetModule().NuGetPackageId, Model.GetModule().NuGetPackageVersion));
            //}
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Partial",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
            );
        }

        public IList<string> FolderBaseList => new[] { "Templates" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Templates") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);
        public string FolderNamespace => string.Join(".", FolderBaseList);

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish("TemplateRegistrationRequired", new Dictionary<string, string>()
            {
                { "TemplateId", GetTemplateId() },
                { "TemplateType", "Java Template" },
                { "Role", GetRole() },
                //{ "Module Dependency", Model.GetDesignerSettings()?.ModuleDependency },
                //{ "Module Dependency Version",Model.GetDesignerSettings()?.ModuleVersion },
                { "ModelId", Model.Id }
            });
        }
        private string GetRole()
        {
            return Model.GetRole() ?? GetTemplateId();
        }

        public string GetTemplateId()
        {
            return $"{Project.Application.Name}.{FolderNamespace}.{Model.Name}";
        }

        private string GetModelType()
        {
            return Model.GetModelName();
        }
    }
}