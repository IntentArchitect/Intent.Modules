using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.CSharp.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial
{
    [IntentManaged(Mode.Merge)]
    partial class CSharpTemplatePartial : CSharpTemplateBase<CSharpTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.CSharp.Templates.CSharpTemplatePartial";

        public CSharpTemplatePartial(IOutputTarget project, CSharpTemplateModel model) : base(TemplateId, project, model)
        {
            AddNugetDependency(NugetPackages.IntentCommonCSharp);
            AddNugetDependency(NugetPackages.IntentRoslynWeaverAttributes);
        }

        public string TemplateName => Model.Name.EndsWith("Template") ? Model.Name : $"{Model.Name}Template";
        public IList<string> OutputFolder => Model.GetFolderPath().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
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
                templateType: "C# Template",
                role: GetRole(),
                location: Model.GetLocation()));

            Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: "Intent.Common.CSharp",
                moduleVersion: "3.0.0-beta.3"));
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

        public string GetTemplateId()
        {
            return $"{Project.Application.Name}.{FolderNamespace}";
        }

        private string GetModelType()
        {
            return Model.GetModelName();
        }
    }
}