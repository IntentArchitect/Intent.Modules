using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Templates;
public interface ICSharpStyleSettings
{
    /// <summary>
    /// Setting to determine the behavior of a constructor initializer (call to "this" or "base"). 
    /// Related to <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1128.md">StyleCop rule SA1128</see>
    /// </summary>
    ConstructorInitializerOptions ConstructorInitializerBehavior { get; init; }

    /// <summary>
    /// Setting to determine the behavior of a parameters
    /// Related to <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1128.md">StyleCop rule SA1116</see>
    /// </summary>
    ParameterPlacementOptions ParameterPlacement { get; init; }

    /// <summary>
    /// The order in which elements in the file should be ordered
    /// </summary>
    IEnumerable<string> ElementOrder { get; }
}
