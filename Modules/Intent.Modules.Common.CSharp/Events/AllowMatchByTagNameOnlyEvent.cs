namespace Intent.Modules.Common.CSharp.Events;

/// <summary>
/// Payload for event fired by <see cref="IRazorTagMatchingConfiguration.AllowMatchByTagNameOnly"/>.
/// </summary>
public class AllowMatchByTagNameOnlyEvent
{
    /// <summary>
    /// The element or directive tag name.
    /// </summary>
    public string TagName { get; init; }
}