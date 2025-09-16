using Intent.Engine;
using Intent.Modules.Common.CSharp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Templates;
internal class CSharpStyleSettings : ICSharpStyleSettings
{
    internal static void RegisterSettings(IApplication application)
    {
        _ = new CSharpStyleSettings(application);
    }

    internal CSharpStyleSettings(IApplication application)
    {
        ConstructorInitializerBehavior = application.Settings.GetCSharpStyleConfiguration()?.ConstructorInitializer();
        ParameterPlacement = application.Settings.GetCSharpStyleConfiguration()?.ParameterPlacement();
        BlankLinesBetweenMembers = application.Settings.GetCSharpStyleConfiguration()?.BlankLinesBetweenMembers();

        Settings = this;
    }

    internal CSharpStyleSettings(ICSharpStyleSettings styleSettings)
    {
        Settings = styleSettings;
    }

    internal static ICSharpStyleSettings Settings { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ConstructorInitializerOptions ConstructorInitializerBehavior { get; init ; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ParameterPlacementOptions ParameterPlacement { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public BlankLinesBetweenMembersOptions BlankLinesBetweenMembers { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> ElementOrder { get; } = ["public", "internal", "protected readonly", "protected internal", "protected", "", "private readonly", "private protected", "private"];
}
