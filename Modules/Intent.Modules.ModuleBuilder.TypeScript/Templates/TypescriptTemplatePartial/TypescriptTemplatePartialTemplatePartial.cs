using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.TypeScript.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial
{
    [IntentManaged(Mode.Merge)]
    partial class TypescriptTemplatePartialTemplate : CSharpTemplateBase<TypescriptFileTemplateModel>, IModuleBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TypescriptTemplatePartialTemplate(IOutputTarget outputTarget, TypescriptFileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency("Intent.Modules.Common.TypeScript", "3.4.3");
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        public IList<string> OutputFolder => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name.ToCSharpIdentifier().RemoveSuffix("Template") }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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
            ExecutionContext.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(this));

            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: "Intent.Common.TypeScript",
                moduleVersion: "3.4.3"));
            if (Model.GetModelType() != null)
            {
                ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private string GetAccessModifier()
        {
            if (Model.GetTypeScriptTemplateSettings()?.TemplatingMethod()?.IsTypeScriptFileBuilder() == true
                || Model.GetTypeScriptTemplateSettings()?.TemplatingMethod()?.IsCustom() == true)
            {
                return "public partial ";
            }
            return "partial ";
        }

        private string GetClassName()
        {
            return $"{(Model.IsFilePerModelTemplateRegistration() ? $"{{Model.Name}}" : Model.Name.RemoveSuffix("Template"))}";
        }

        private IEnumerable<string> GetBaseTypes()
        {
            if (Model.DecoratorContract != null)
            {
                yield return $"TypeScriptTemplateBase<{GetModelType()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            else
            {
                yield return $"TypeScriptTemplateBase<{GetModelType()}>";
            }

            if (Model.GetTypeScriptTemplateSettings().TemplatingMethod().IsTypeScriptFileBuilder())
            {
                yield return nameof(ITypescriptFileBuilderTemplate);
            }
        }

        public string GetRole() => Model.GetRole();

        public string TemplateType() => "Typescript Template";

        public string GetTemplateId() => $"{Model.GetModule().Name}.{FolderNamespace}";

        public string GetDefaultLocation() => Model.GetLocation();

        public string GetModelType() => Model.GetModelName();
    }
}