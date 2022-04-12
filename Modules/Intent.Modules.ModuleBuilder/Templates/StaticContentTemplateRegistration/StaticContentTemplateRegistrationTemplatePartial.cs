using System.Collections.Generic;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.StaticContentTemplateRegistration
{
    partial class StaticContentTemplateRegistrationTemplate : CSharpTemplateBase<StaticContentTemplateModel>, IModuleBuilderTemplate
    {
        private const string AdditionalFolderName = "StaticContentTemplateRegistrations";
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration";
        private static readonly string[] SuffixesToRemove = { "Content", "Template", "Registration", "Registrations" };

        [IntentManaged(Mode.Merge)]
        public StaticContentTemplateRegistrationTemplate(IOutputTarget outputTarget, StaticContentTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(this));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            var name = Model.Name
                .ToCSharpIdentifier()
                .EnsureSuffixedWith("StaticContentTemplateRegistration", SuffixesToRemove);

            return new CSharpFileConfig(
                className: name,
                @namespace: $"{this.GetNamespace(AdditionalFolderName)}",
                relativeLocation: $"{this.GetFolderPath(AdditionalFolderName)}",
                fileName: name);
        }

        public string GetTemplateId() => $"{Namespace}.{ClassName}";

        public string GetModelType() => null;

        public string GetRole() => Model.GetTemplateSettings().Role();

        public string TemplateType() => "Static Content Template";

        public string GetDefaultLocation() => Model.GetTemplateSettings().DefaultLocation();
    }
}