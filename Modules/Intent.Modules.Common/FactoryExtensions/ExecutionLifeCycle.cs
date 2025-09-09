using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Plugins;

namespace Intent.Modules.Common.FactoryExtensions;

/// <summary>
/// A static action registry that connects to the Execution Life Cycle of the Software Factory.
/// </summary>
public class ExecutionLifeCycle : FactoryExtensionBase
{
    public override string Id => "Intent.Common.CacheClearingFactoryExtension";

    public override int Order => int.MinValue; // always execute first

    private static IList<Action> _onStartActions = new List<Action>();

    /// <summary>
    /// Registers an action to be executed on start of the execution.
    /// <remarks>
    /// This can be used to clear static caches between execution runs to prevent cache spillover between them.
    /// </remarks>
    /// </summary>
    /// <param name="action"></param>
    public static void OnStart(Action action)
    {
        _onStartActions.Add(action);
    }

    /// <inheritdoc />
    protected override void OnStart(IApplication application)
    {
        base.OnStart(application);
        foreach (var action in _onStartActions)
        {
            action();
        }
    }
}