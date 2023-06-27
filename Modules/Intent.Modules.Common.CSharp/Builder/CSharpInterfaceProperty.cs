using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceProperty : CSharpProperty
{
    public CSharpInterfaceProperty(string type, string name) : base(type, name, null)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        AccessModifier = string.Empty;
    }

    public CSharpInterfaceProperty Public()
    {
        AccessModifier = "public ";
        return this;
    }
}