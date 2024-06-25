using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

// See sample as to how this was named
// https://sharplab.io/#v2:C4LghgzsA0AmIGoA+ABATARgLACgUAYACFDAFgG5cUBmYtQgYUIG9dD3jaVTCBZACgCULNhzEB6cYQAiAUwDGAGzAAnMMACWAewB2hAKIAPAA4rZECNr0B3DcAAWhAAqrZOh+Y0AvWbABqqhpgAEaKsnKWAOY66laiYuwAbqqE/DEAtrLQhBAArioZssIAvIQA4rLAAHJgmUKUOAkckoQAKrnGYW0AnsayhAEqQaH9ckqqsbrxCfwk+NlzwtW1/aUVy3WCDU3sLQCSsG6aAGYasio9nf2Dw11jymqaU407yRcbsvRrlTWb2zstADKGh0kS6NxCdwUD0mOmmYjmhEOEHkhG+wAi8iGxieOnq8I4BKSKVgGnkwDRhB0smsMjJuNU3QAPHMFhh8AA+fEvJote4TXEGExmCxWQggyyHQgAQRUkVymXchFsDkIACVZMcAPIqLW5YAAaVk3WsWhUsCJSPpADpWipuusAopcrJ+AAiN3ZLT6whvX1gZ1Ff7sAC+02mNFSiN+WWI7MIgPyhWE6xjQkI01YPISKAA7Kk3QApLT2HSewhu6RaWRurbTMPZiNcePrTHY3HpzOWvMV6U6XTddLeiBu4OEENiXAhoA===

public class CSharpVariableDeclaration : CSharpStatement
{
    public CSharpVariableDeclaration(string variableDeclaration) : this(null, variableDeclaration)
    {
    }

    public CSharpVariableDeclaration(string? type, string variableDeclaration)
    {
        Type = string.IsNullOrWhiteSpace(type) ? "var" : type;
        VariableDeclaration = variableDeclaration;
        Text = $"{Type} {variableDeclaration}";
    }

    public string Type { get; }
    public string VariableDeclaration { get; }
}

public class CSharpDeclarationExpression : CSharpStatement
{
    public CSharpDeclarationExpression(IReadOnlyList<string> designatedVariables)
        : base($"var ({string.Join(", ", designatedVariables)})")
    {
        DesignatedVariables = designatedVariables;
    }

    public IReadOnlyList<string> DesignatedVariables { get; }
}