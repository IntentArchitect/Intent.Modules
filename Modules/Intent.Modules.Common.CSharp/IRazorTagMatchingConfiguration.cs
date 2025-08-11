using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Intent.Modules.Common.CSharp;

/// <summary>
/// A fluent style configurator for <see cref="RazorConfigurationExtensions.ConfigureRazorTagMatchingFor"/>.
/// </summary>
public interface IRazorTagMatchingConfiguration
{
    /// <summary>
    /// Allows the tag to be matched by any direct descendant within the specified <paramref name="path"/>.
    /// </summary>
    IRazorTagMatchingConfiguration AllowMatchByDescendant(IReadOnlyList<string> path);

    /// <summary>
    /// Adds an item to the Razor Weaver's list of element/directive tag names which may be matched
    /// by tag name alone rather than requiring content also be matched.
    /// </summary>
    IRazorTagMatchingConfiguration AllowMatchByNameOnly();

    /// <summary>
    /// Adds entries to the Razor Weaver's list of attributes for a tag name which can be used
    /// to find matches of elements/directives between existing and generated files.
    /// </summary>
    IRazorTagMatchingConfiguration AllowMatchByAttributes(params string[] attributeNames);
}