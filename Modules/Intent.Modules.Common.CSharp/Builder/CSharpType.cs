using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public abstract class CSharpType
{
    internal const string TaskShortTypeName = "Task";
    internal const string TaskFullTypeName = "System.Threading.Tasks.Task";

    public static CSharpTypeName CreateTask(ICSharpTemplate? template = null)
    {
        return new CSharpTypeName(template?.UseType(TaskFullTypeName) ?? TaskFullTypeName);
    }

    public static CSharpTypeGeneric CreateTask(CSharpType genericParamType, ICSharpTemplate? template = null)
    {
        return new CSharpTypeGeneric(template?.UseType(TaskFullTypeName) ?? TaskFullTypeName, [genericParamType]);
    }

    public static CSharpTypeVoid CreateVoid()
    {
        return new CSharpTypeVoid();
    }
}

public class CSharpTypeVoid : CSharpType
{
    public CSharpTypeVoid()
    {
    }

    public override string ToString()
    {
        return "void";
    }

    public override bool Equals(object? obj)
    {
        return obj is CSharpTypeVoid;
    }
}

public class CSharpTypeName : CSharpType
{
    public CSharpTypeName(string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        TypeName = typeName;
    }

    public string TypeName { get; }

    public override string ToString()
    {
        return TypeName;
    }

    protected bool Equals(CSharpTypeName other)
    {
        return TypeName == other.TypeName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeName)obj);
    }

    public override int GetHashCode()
    {
        return TypeName.GetHashCode();
    }
}

public class CSharpTypeGeneric : CSharpType
{
    public CSharpTypeGeneric(string typeName, IReadOnlyList<CSharpType> typeArgumentList)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        ArgumentNullException.ThrowIfNull(typeArgumentList);

        TypeName = typeName;
        TypeArgumentList = typeArgumentList;
    }

    public string TypeName { get; }
    public IReadOnlyList<CSharpType> TypeArgumentList { get; }

    public override string ToString()
    {
        return $"{TypeName}<{string.Join(", ", TypeArgumentList.Select(s => s.ToString()))}>";
    }

    protected bool Equals(CSharpTypeGeneric other)
    {
        return TypeName == other.TypeName && TypeArgumentList.SequenceEqual(other.TypeArgumentList);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeGeneric)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TypeName, TypeArgumentList);
    }
}

public class CSharpTupleElement
{
    public CSharpTupleElement(CSharpType type, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(type);
        Type = type;
        Name = name;
    }
    
    public CSharpType Type { get; }
    public string? Name { get; }

    protected bool Equals(CSharpTupleElement other)
    {
        return Type.Equals(other.Type) && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTupleElement)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Name);
    }
}

public class CSharpTypeTuple : CSharpType
{
    public CSharpTypeTuple(IReadOnlyList<CSharpTupleElement> elements)
    {
        ArgumentNullException.ThrowIfNull(elements);
        Elements = elements;
    }

    public IReadOnlyList<CSharpTupleElement> Elements { get; }

    public override string ToString()
    {
        return $"({string.Join(", ", Elements.Select(s => !string.IsNullOrWhiteSpace(s.Name) ? $"{s.Type} {s.Name}" : s.Type.ToString()))})";
    }

    protected bool Equals(CSharpTypeTuple other)
    {
        return Elements.SequenceEqual(other.Elements);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeTuple)obj);
    }

    public override int GetHashCode()
    {
        return Elements.GetHashCode();
    }
}

public class CSharpTypeArray : CSharpType
{
    public CSharpTypeArray(CSharpType elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType);
        ElementType = elementType;
    }
    
    public CSharpType ElementType { get; }

    public override string ToString()
    {
        return $"{ElementType}[]";
    }

    protected bool Equals(CSharpTypeArray other)
    {
        return ElementType.Equals(other.ElementType);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeArray)obj);
    }

    public override int GetHashCode()
    {
        return ElementType.GetHashCode();
    }
}

public class CSharpTypeNullable : CSharpType
{
    public CSharpTypeNullable(CSharpType elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType);
        ElementType = elementType;
    }
    
    public CSharpType ElementType { get; }

    public override string ToString()
    {
        return $"{ElementType}?";
    }

    protected bool Equals(CSharpTypeNullable other)
    {
        return ElementType.Equals(other.ElementType);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CSharpTypeNullable)obj);
    }

    public override int GetHashCode()
    {
        return ElementType.GetHashCode();
    }
}

public static class CSharpTypeExtensions
{
    public static bool IsTask(this CSharpType type)
    {
        return (type is CSharpTypeName name && name.IsTask()) || (type is CSharpTypeGeneric generic && generic.IsTask());
    }
    
    public static bool IsTask(this CSharpTypeName name)
    {
        return name.TypeName is CSharpType.TaskFullTypeName or CSharpType.TaskShortTypeName;
    }
    
    public static bool IsTask(this CSharpTypeGeneric generic)
    {
        return generic.TypeName is CSharpType.TaskFullTypeName or CSharpType.TaskShortTypeName;
    }
    
    public static CSharpType WrapInTask(this CSharpType type, ICSharpTemplate? template = null)
    {
        if (type is CSharpTypeVoid)
        {
            return CSharpType.CreateTask(template);
        }

        return CSharpType.CreateTask(type, template);
    }

    public static CSharpType? GetTaskGenericType(this CSharpType type)
    {
        if (!type.IsTask())
        {
            return null;
        }

        return (type as CSharpTypeGeneric)?.TypeArgumentList.FirstOrDefault();
    }
}