using System;

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpInterfaceProperty : CSharpProperty
{
    public CSharpInterfaceProperty(string type, string name) : this(type, name, null)
    {
    }

    public CSharpInterfaceProperty(string type, string name, CSharpInterface parent) : base(type, name, null)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cannot be null or empty", nameof(name));
        }

        Parent = parent;
        File = parent.File;
        AccessModifier = string.Empty;
    }

    public CSharpInterfaceProperty Public()
    {
        AccessModifier = "public ";
        return this;
    }
}