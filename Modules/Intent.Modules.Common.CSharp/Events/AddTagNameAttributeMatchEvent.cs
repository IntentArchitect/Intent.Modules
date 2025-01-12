namespace Intent.Modules.Common.CSharp.Events;

/// <summary>
/// Payload for event fired by <see cref="IRazorConfigurator.AllowMatchByTagNameOnly"/>.
/// </summary>
public class AddTagNameAttributeMatchEvent
{
    /// <summary>
    /// Payload for event fired by <see cref="IRazorConfigurator.AllowMatchByTagNameOnly"/>.
    /// </summary>
    public string TagName { get; init; }

    /// <summary>
    /// The attribute name on the <see cref="TagName"/>.
    /// </summary>
    public string AttributeName { get; init; }
}