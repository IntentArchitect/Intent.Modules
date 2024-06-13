using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpVariableDeclaration : CSharpStatement
{
    public CSharpVariableDeclaration(string variableDeclaration)
    {
        VariableDeclaration = variableDeclaration;
        Text = $"var {variableDeclaration}";
    }
    
    public string VariableDeclaration { get; }
}

public class CSharpDeconstructedVariableDeclaration : CSharpVariableDeclaration
{
    public CSharpDeconstructedVariableDeclaration(IReadOnlyCollection<string> deconstructedFieldNames)
        : base($"({string.Join(", ", deconstructedFieldNames)})")
    {
        DeconstructedFieldNames = deconstructedFieldNames;
    }

    public IReadOnlyCollection<string> DeconstructedFieldNames { get; }
}