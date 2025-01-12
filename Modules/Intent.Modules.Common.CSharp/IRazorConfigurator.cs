namespace Intent.Modules.Common.CSharp;

/// <summary>
/// A fluent style Razor configurator.
/// </summary>
public interface IRazorConfigurator
{
    /// <summary>
    /// Adds an item to the Razor Weaver's list of element/directive tag names which may be matched
    /// by tag name alone rather than requiring content also be matched.
    /// </summary>
    /// <remarks>
    /// Used by the Razor Weaver when determining matches of element/directives between existing
    /// and generated files.
    /// </remarks>
    IRazorConfigurator AllowMatchByTagNameOnly(string tagName);

    /// <summary>
    /// Adds an entry to the Razor Weaver's list of attributes for a tag name which can be used
    /// to find matches of elements/directives between existing and generated files.
    /// </summary>
    IRazorConfigurator AddTagNameAttributeMatch(string tagName, string attributeName);
}