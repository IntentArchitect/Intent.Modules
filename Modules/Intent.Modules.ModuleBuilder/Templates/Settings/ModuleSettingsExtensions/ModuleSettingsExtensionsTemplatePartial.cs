using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.ModuleBuilder.Api;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ModuleSettingsExtensionsTemplate : CSharpTemplateBase<IList<ModuleSettingsConfigurationModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ModuleSettingsExtensionsTemplate(IOutputTarget outputTarget, IList<ModuleSettingsConfigurationModel> model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ModuleSettingsExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}