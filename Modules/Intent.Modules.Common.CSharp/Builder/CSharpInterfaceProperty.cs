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
    }

    public override string GetText(string indentation)
    {
        return $@"{(!XmlComments.IsEmpty() ? $@"{XmlComments.ToString(indentation)}
" : string.Empty)}{indentation}{Type} {Name} {{ {Getter}{(!IsReadOnly ? $" {Setter}" : string.Empty)} }}{(InitialValue != null ? $" = {InitialValue};" : string.Empty)}";
    }
}