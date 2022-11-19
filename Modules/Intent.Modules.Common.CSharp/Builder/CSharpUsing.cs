using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpUsing
{
    public string Namespace { get; }

    public CSharpUsing(string @namespace)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(@namespace));
        }

        Namespace = @namespace;
    }

    public override string ToString()
    {
        return $"using {Namespace};";
    }
}