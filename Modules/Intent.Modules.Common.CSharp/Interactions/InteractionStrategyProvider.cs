#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.Interactions;

public class InteractionStrategyProvider
{
    private readonly List<(IInteractionStrategy Strategy, int Priority)> _strategies = [];
    public static readonly InteractionStrategyProvider Instance = new();

    static InteractionStrategyProvider()
    {
        ExecutionLifeCycle.OnStart(() =>
        {
            Instance._strategies.Clear();
        });
    }

    public void Register(IInteractionStrategy strategy) => Register(strategy, priority: 0);

    public void Register(IInteractionStrategy strategy, int priority)
    {
        _strategies.Add((strategy, priority));
    }

    public bool HasInteractionStrategy(IElement interaction)
    {
        var matched = _strategies.Where(x => x.Strategy.IsMatch(interaction)).ToList();

        if (matched.Count > 1)
        {
            Logging.Log.Debug($"Multiple interaction strategies matched for {interaction}: [{string.Join(", ", matched)}]");
        }

        return matched.Count > 0;
    }

    public IInteractionStrategy? GetInteractionStrategy(IElement interaction)
    {
        var matched = _strategies
            .OrderBy(x => x.Priority)
            .Where(x => x.Strategy.IsMatch(interaction)).ToList();

        if (matched.Count == 0)
        {
            Logging.Log.Warning($"No interaction strategy matched for {interaction}");
            return null;
        }

        if (matched.Count > 1)
        {
            Logging.Log.Debug($@"Multiple interaction strategies found for {interaction}: [{string.Join(", ", matched.Select(x => x.Strategy.GetType().Name))}]
Choosing highest priority: {matched[0].Strategy.GetType().Name}");
        }

        return matched[0].Strategy;
    }
}