using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Events;

/// <summary>
/// Payload for event fired by <see cref="IRazorTagMatchingConfiguration.AllowMatchByDescendant"/>.
/// </summary>
public class AllowMatchByDescendantEvent
{
    /// <summary>
    /// The element or directive tag name.
    /// </summary>
    public string TagName { get; init; }

    /// <summary>
    /// The descendant path which is allowed to be matched.
    /// </summary>
    public IReadOnlyList<string> Path { get; init; }
}