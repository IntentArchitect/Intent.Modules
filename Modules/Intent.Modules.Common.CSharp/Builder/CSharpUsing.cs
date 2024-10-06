using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpUsing
{
    public string Namespace { get; }

    public bool IsGlobal { get; }

    public CSharpUsing(string @namespace, bool isGlobal)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(@namespace));
        }

        Namespace = @namespace;
    }

    public CSharpUsing(string @namespace) : this(@namespace, isGlobal: false)
    {
    }

    public override string ToString()
    {
        return $"{(IsGlobal ? "global " : string.Empty)}using {Namespace};";
    }
}