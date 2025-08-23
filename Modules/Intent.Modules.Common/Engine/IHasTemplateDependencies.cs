#pragma warning disable IDE0130
using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common;

/// <summary>
/// Has dependencies on other <see cref="ITemplate"/> instances.
/// </summary>
public interface IHasTemplateDependencies
{
    /// <summary>
    /// Gets all the <see cref="ITemplateDependency"/> items for this template..
    /// </summary>
    IEnumerable<ITemplateDependency> GetTemplateDependencies();
}