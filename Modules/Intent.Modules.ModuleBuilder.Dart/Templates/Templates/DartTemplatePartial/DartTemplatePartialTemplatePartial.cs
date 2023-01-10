using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.ModuleBuilder.Dart.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Dart.Templates.Templates.DartTemplatePartial
{
    [IntentManaged(Mode.Merge)]
    partial class DartTemplatePartialTemplate : CSharpTemplateBase<DartFileTemplateModel>, IModuleBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.ModuleBuilder.Dart.Templates.DartTemplatePartial";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DartTemplatePartialTemplate(IOutputTarget outputTarget, DartFileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            // whichever his higher:
            AddNugetDependency("Intent.Modules.Common.Dart", "1.0.0");
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
                moduleId: "Intent.Common.Dart",
                moduleVersion: "1.0.0"));
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
                return $"DartTemplateBase<{GetModelType()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            return $"DartTemplateBase<{GetModelType()}>";
        }

        public string GetRole() => Model.GetRole();

        public string TemplateType() => "Dart Template";

        public string GetTemplateId() => $"{Model.GetModule().Name}.{FolderNamespace}";

        public string GetDefaultLocation() => Model.GetLocation();

        public string GetModelType() => Model.GetModelName();
    }
}