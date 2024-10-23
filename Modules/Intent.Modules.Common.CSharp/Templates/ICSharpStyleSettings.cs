using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Templates;
public interface ICSharpStyleSettings
{
    ConstructorInitializerOptions ConstructorInitializerBehavior { get; init; }

    IEnumerable<string> ElementOrder { get; }
}
