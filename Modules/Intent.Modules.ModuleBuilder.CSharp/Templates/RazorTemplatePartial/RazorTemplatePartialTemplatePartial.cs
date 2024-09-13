using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.RazorTemplatePartial
{
    [IntentManaged(Mode.Merge)]
    partial class RazorTemplatePartialTemplate : CSharpTemplateBase<RazorTemplateModel>, IModuleBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorTemplatePartialTemplate(IOutputTarget outputTarget, RazorTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(CSharp.NugetPackages.IntentModulesCommonCSharp(outputTarget));
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.Name.ToCSharpIdentifier().RemoveSuffix("Template"))}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.Name.ToCSharpIdentifier().RemoveSuffix("Template"))}",
                fileName: $"{TemplateName}Partial");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(this));

            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: IntentModule.IntentCommon.Name,
                moduleVersion: IntentModule.IntentCommon.Version));

            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: IntentModule.IntentCommonCSharp.Name,
                moduleVersion: IntentModule.IntentCommonCSharp.Version));

            if (Model.GetDesigner() != null)
            {
                ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetDesigner().ParentModule.Name,
                    moduleVersion: Model.GetDesigner().ParentModule.Version));
            }

            if (Model.GetModelType() != null)
            {
                ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private IEnumerable<string> GetBaseTypes()
        {
            yield return $"RazorTemplateBase<{GetModelType()}>";

            if (Model.GetRazorTemplateSettings().TemplatingMethod().IsRazorFileBuilder())
            {
                yield return UseType("Intent.Modules.Blazor.Api.IRazorFileTemplate");
            }
        }

        private string GetAccessModifier()
        {
            if (Model.GetRazorTemplateSettings()?.TemplatingMethod()?.IsRazorFileBuilder() == true)
            {
                return "public ";
            }

            return "partial ";
        }

        private string GetModelType() => NormalizeNamespace(Model.GetModelName());

        private string GetClassName()
        {
            return $"{(Model.IsFilePerModelTemplateRegistration() ? "{Model.Name}" : Model.Name.RemoveSuffix("Template", "Interface"))}";
        }

        string IModuleBuilderTemplate.GetModelType() => Model.GetModelName();

        string IModuleBuilderTemplate.GetRole() => Model.GetRole();

        string IModuleBuilderTemplate.TemplateType() => "Razor Template";

        string IModuleBuilderTemplate.GetDefaultLocation() => Model.GetLocation();

        public string GetTemplateId() => $"{Model.GetModule().Name}.{string.Join(".", Model.GetParentFolderNames().Append(Model.Name))}";
    }
}