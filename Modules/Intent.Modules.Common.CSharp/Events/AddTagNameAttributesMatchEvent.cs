namespace Intent.Modules.Common.CSharp.Events;

/// <summary>
/// Payload for event fired by <see cref="IRazorTagMatchingConfiguration.AllowMatchByTagNameOnly"/>.
/// </summary>
public class AddTagNameAttributesMatchEvent
{
    /// <summary>
    /// Payload for event fired by <see cref="IRazorTagMatchingConfiguration.AllowMatchByTagNameOnly"/>.
    /// </summary>
    public string TagName { get; init; }

    /// <summary>
    /// The attribute names on the <see cref="TagName"/>.
    /// </summary>
    public string[] AttributeNames { get; init; }
}