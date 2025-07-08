#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.Interactions;

public class InteractionStrategyProvider
{
    private readonly IList<IInteractionStrategy> _strategies = [];
    public static readonly InteractionStrategyProvider Instance = new();

    public void Register(IInteractionStrategy strategy)
    {
        _strategies.Add(strategy);
    }

    public bool HasInteractionStrategy(IElement interaction)
    {
        var matched = _strategies.Where(x => x.IsMatch(interaction)).ToList();

        if (matched.Count > 1)
        {
            Logging.Log.Debug($"Multiple interaction strategies matched for {interaction}: [{string.Join(", ", matched)}]");
        }

        return matched.Count > 0;
    }

    public IInteractionStrategy? GetInteractionStrategy(IElement interaction)
    {
        var matched = _strategies.Where(x => x.IsMatch(interaction)).ToList();
        if (matched.Count == 0)
        {
            Logging.Log.Warning($"No interaction strategy matched for {interaction}");
            return null;
        }

        if (matched.Count > 1)
        {
            Logging.Log.Debug($"Multiple interaction strategies found for {interaction}: [{string.Join(", ", matched)}]");
        }

        return matched[0];
    }
}