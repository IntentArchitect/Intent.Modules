using Intent.Modules.Common.CSharp.Settings;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Common.CSharp.Settings.CSharpStyleConfiguration;

namespace Intent.Modules.Common.CSharp.Tests;
internal class TestStyleSettings : ICSharpStyleSettings
{
    public TestStyleSettings(string constructorInitValue, string paramNewLineValue, string blankLinesValue)
    {
        ConstructorInitializerBehavior = new ConstructorInitializerOptions(constructorInitValue);
        ParameterPlacement = new ParameterPlacementOptions(paramNewLineValue);
        BlankLinesBetweenMembers = new BlankLinesBetweenMembersOptions(blankLinesValue);
    }

    public ConstructorInitializerOptions ConstructorInitializerBehavior { get; init; }

    public ParameterPlacementOptions ParameterPlacement { get; init; }
    
    public BlankLinesBetweenMembersOptions BlankLinesBetweenMembers { get; init; }

    public IEnumerable<string> ElementOrder { get; } = ["public", "internal", "protected readonly", "protected internal", "protected", "", "private readonly", "private protected", "private"];
    
}
