using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Html.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Html.Templates.HtmlFileTemplatePartial
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class HtmlFileTemplatePartialTemplate : CSharpTemplateBase<HtmlFileTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Html.Templates.HtmlFileTemplatePartial";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public HtmlFileTemplatePartialTemplate(IOutputTarget outputTarget, HtmlFileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentCommonHtml);
            //if (!string.IsNullOrWhiteSpace(Model.GetModule().NuGetPackageId))
            //{
            //    AddNugetDependency(new NugetPackageInfo(Model.GetModule().NuGetPackageId, Model.GetModule().NuGetPackageVersion));
            //}
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                fileName: $"{TemplateName}Partial",
                relativeLocation: $"{FolderPath}");
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(
                modelId: Model.Id,
                templateId: GetTemplateId(),
                templateType: "Html Template",
                role: GetRole(),
                location: Model.GetLocation()));
            Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: "Intent.Common.Html",
                moduleVersion: "3.0.2"));
            if (Model.GetModelType() != null)
            {
                Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }
        private string GetRole()
        {
            return Model.GetRole();
        }

        public string GetTemplateId()
        {
            return $"{Model.GetModule().Name}.{FolderNamespace}";
        }

        private string GetModelType()
        {
            return Model.GetModelName();
        }
    }
}