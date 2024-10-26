using Intent.Engine;
using Intent.Modules.Common.CSharp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Templates;
internal class CSharpStyleSettings : ICSharpStyleSettings
{
    internal static void RegisterSettings(IApplication application)
    {
        _ = new CSharpStyleSettings(application);
    }

    internal CSharpStyleSettings(IApplication application)
    {
        ConstructorInitializerBehavior = application.Settings.GetCSharpStyleConfiguration()?.ConstructorInitializer();
        
        Settings = this;
    }

    internal static ICSharpStyleSettings Settings { get; private set; }

    /// <summary>
    /// Setting to deetermine the behavior of a constructor initializer (call to "this" or "base"). 
    /// Related to <see href="https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1128.md">StyleCop rule SA1128</see>
    /// </summary>
    public ConstructorInitializerOptions ConstructorInitializerBehavior { get; init ; }

    public IEnumerable<string> ElementOrder { get; } = ["public", "internal", "protected readonly", "protected internal", "protected", "", "private readonly", "private protected", "private"];
}
