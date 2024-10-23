using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CSharpStyleSettingsLoaderExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.CSharp.CSharpStyleSettingsLoaderExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => -100;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            CSharpStyleSettings.RegisterSettings(application);
        }
    }
}