using System;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public class HtmlAttribute : IEquatable<HtmlAttribute>
{
    public HtmlAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; }
    public string Value { get; set; }

    public bool Equals(HtmlAttribute other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((HtmlAttribute)obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }
}