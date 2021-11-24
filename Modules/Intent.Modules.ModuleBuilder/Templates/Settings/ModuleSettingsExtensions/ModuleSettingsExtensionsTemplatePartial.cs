using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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


        public override bool CanRunTemplate()
        {
            return Model.Any();
        }
    }
}