using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.CSharp.FactoryExtensions;

public class CSharpFileBuilderFactoryExtension : FactoryExtensionBase
{
    /// <inheritdoc />
    public override string Id => "Intent.Common.CSharp.CSharpFileBuilderFactoryExtension";

    /// <inheritdoc />
    public override int Order => 100_000_000; // always execute last

    /// <inheritdoc />
    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        FileBuilderHelper.PerformConfiguration<ICSharpFileBuilderTemplate>(
            context: application,
            getActions: template =>
            {
                template.CSharpFile.Template ??= template;

                return template.CSharpFile.GetConfigurationDelegates();
            },
            afterAllComplete: t => t.CSharpFile.MarkCompleteBuildAsDone());
    }

    /// <inheritdoc />
    protected override void OnBeforeTemplateExecution(IApplication application)
    {
        FileBuilderHelper.PerformConfiguration<ICSharpFileBuilderTemplate>(
            context: application,
            getActions: t => t.CSharpFile.GetConfigurationAfterDelegates(),
            afterAllComplete: t => t.CSharpFile.MarkAfterBuildAsDone());
    }
}