using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.FactoryExtensions
{
    public class FileBuilderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Common.FileBuilderFactoryExtension";

        public override int Order => int.MaxValue; // always execute last

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<IFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x =>
            {
                x.File.StartBuild();
            });

            templates.ForEach(x =>
            {
                x.File.CompleteBuild();
            });
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
                .Where(x => x.CanRunTemplate())
                .OfType<IFileBuilderTemplate>()
                .ToList();

            templates.ForEach(x =>
            {
                x.File.AfterBuild();
            });
        }
    }
}