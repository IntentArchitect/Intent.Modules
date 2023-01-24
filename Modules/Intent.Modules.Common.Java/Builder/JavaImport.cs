using System;

namespace Intent.Modules.Common.Java.Builder;

public class JavaImport
{
    public string Namespace { get; }

    public JavaImport(string @namespace)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(@namespace));
        }

        Namespace = @namespace;
    }

    public override string ToString()
    {
        return $"import {Namespace};";
    }
}