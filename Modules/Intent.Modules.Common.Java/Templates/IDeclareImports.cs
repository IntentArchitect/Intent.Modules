using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Templates;

/// <summary>
/// The implemented type declares imports.
/// </summary>
public interface IDeclareImports
{
    /// <summary>
    /// The imports declared, must in the format of the fully qualified type with no "import"
    /// or ";" in it.
    /// </summary>
    IEnumerable<string> DeclareImports();
}