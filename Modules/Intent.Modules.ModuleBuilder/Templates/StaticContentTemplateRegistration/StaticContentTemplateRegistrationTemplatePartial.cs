using System;
using System.Collections.Generic;
using System.Linq;
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
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    partial class StaticContentTemplateRegistrationTemplate : CSharpTemplateBase<StaticContentTemplateModel>, IModuleBuilderTemplate
    {
        private const string AdditionalFolderName = "StaticContentTemplateRegistrations";
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration";
        private static readonly string[] SuffixesToRemove = { "Content", "Template", "Registration", "Registrations" };

        [IntentManaged(Mode.Merge)]
        public StaticContentTemplateRegistrationTemplate(IOutputTarget outputTarget, StaticContentTemplateModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(this));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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

        public string GetGlobStrings(string binaryGlobbingPattern)
        {
            var sp = binaryGlobbingPattern.Replace("\r\n", "\n").Split("\n");
            return string.Join(", ", sp.Select(x => $"\"{x}\""));
        }

        public string GetTemplateId() => $"{Namespace}.{ClassName}";

        public string GetModelType() => null;

        public string GetRole() => Model.GetTemplateSettings().Role();

        public string TemplateType() => "Static Content Template";

        public string GetDefaultLocation() => Model.GetTemplateSettings().DefaultLocation();
    }
}