using Intent.Engine;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.FactoryExtensions
{
    public class FileBuilderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.FileBuilderFactoryExtension";

        public override int Order => int.MaxValue; // always execute last

        /// <inheritdoc />
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            FileBuilderHelper.PerformConfiguration(application, isForAfterBuild: false);
        }

        /// <inheritdoc />
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            FileBuilderHelper.PerformConfiguration(application, isForAfterBuild: true);
        }
    }
}