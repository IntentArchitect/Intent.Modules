using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.Java.FactoryExtensions;

public class JavaFileBuilderFactoryExtension: FactoryExtensionBase
{
    public override string Id => "Intent.Common.Java.JavaFileBuilderFactoryExtension";

    public override int Order => int.MaxValue; // always execute last

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
            .OfType<IJavaFileBuilderTemplate>()
            .ToList();

        templates.ForEach(x => x.JavaFile.StartBuild());
        templates.ForEach(x => x.JavaFile.CompleteBuild());
    }

    protected override void OnBeforeTemplateExecution(IApplication application)
    {
        var templates = application.OutputTargets.SelectMany(x => x.TemplateInstances)
            .OfType<IJavaFileBuilderTemplate>()
            .ToList();

        templates.ForEach(x => x.JavaFile.AfterBuild());
    }
}