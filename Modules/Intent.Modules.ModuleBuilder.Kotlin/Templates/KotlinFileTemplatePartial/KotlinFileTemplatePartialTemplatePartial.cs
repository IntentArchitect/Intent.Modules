using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Kotlin.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class KotlinFileTemplatePartialTemplate : CSharpTemplateBase<KotlinFileTemplateModel>, IModuleBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Kotlin.Templates.KotlinFileTemplatePartial";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public KotlinFileTemplatePartialTemplate(IOutputTarget outputTarget, KotlinFileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommonKotlin);
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
                moduleId: IntentModule.IntentCommonKotlin.Name,
                moduleVersion: IntentModule.IntentCommonKotlin.Version));
            ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                moduleId: IntentModule.IntentCodeWeavingKotlin.Name,
                moduleVersion: IntentModule.IntentCodeWeavingKotlin.Version));
            if (Model.GetModelType() != null)
            {
                ExecutionContext.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private string GetBaseType()
        {
            if (Model.DecoratorContract != null)
            {
                return $"KotlinTemplateBase<{Model.GetModelName()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            return $"KotlinTemplateBase<{Model.GetModelName()}>";
        }

        public string TemplateType()
        {
            return "Kotlin Template";
        }

        public string GetDefaultLocation()
        {
            return Model.GetLocation();
        }

        public string GetTemplateId()
        {
            return $"{Model.GetModule().Name}.{FolderNamespace}";
        }

        public string GetRole()
        {
            return Model.GetRole();
        }

        public string GetModelType()
        {
            return Model.GetModelName();
        }
    }
}