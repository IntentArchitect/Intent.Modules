using Intent.Engine;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.CSharp.FactoryExtensions;

public class RazorFileBuilderFactoryExtension : FactoryExtensionBase
{
    /// <inheritdoc />
    public override string Id => "Intent.Common.CSharp.RazorFileBuilderFactoryExtension";

    /// <inheritdoc />
    public override int Order => 100_000_000; // always execute last

    /// <inheritdoc />
    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        FileBuilderHelper.PerformConfiguration<IRazorFileTemplate>(
            context: application,
            getActions: template => template.RazorFile.GetConfigurationDelegates(),
            afterAllComplete: t => t.RazorFile.MarkCompleteBuildAsDone());
    }

    /// <inheritdoc />
    protected override void OnBeforeTemplateExecution(IApplication application)
    {
        FileBuilderHelper.PerformConfiguration<IRazorFileTemplate>(
            context: application,
            getActions: t => t.RazorFile.GetConfigurationAfterDelegates(),
            afterAllComplete: t => t.RazorFile.MarkAfterBuildAsDone());
    }
}