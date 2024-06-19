using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.CSharp.FactoryExtensions;

/// <summary>
/// Additional work needed to be performed for all <see cref="ICSharpFileBuilderTemplate"/> instances.
/// </summary>
public class CSharpFileBuilderFactoryExtension : FactoryExtensionBase
{
    /// <inheritdoc />
    public override string Id => "Intent.Common.CSharp.CSharpFileBuilderFactoryExtension";

    /// <inheritdoc />
    public override int Order => int.MinValue;

    /// <summary>
    /// For backwards compatibility ensures that <see cref="CSharpFile.Template"/> is set.
    /// </summary>
    /// <remarks>
    /// All other file builder actions are performed by <see cref="FileBuilderHelper"/> in the
    /// Intent.Common module.
    /// </remarks>
    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var templates = application.OutputTargets
            .SelectMany(x => x.TemplateInstances)
            .OfType<ICSharpFileBuilderTemplate>()
            .ToArray();

        foreach (var template in templates)
        {
            template.CSharpFile.Template ??= template;
        }
    }
}