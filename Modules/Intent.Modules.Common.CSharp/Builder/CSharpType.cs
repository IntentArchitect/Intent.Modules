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
    internal const string TaskFullTypeName = "System.Threading.Task";

    public static CSharpTypeName CreateTask()
    {
        return new CSharpTypeName(TaskFullTypeName);
    }
    
    public static CSharpTypeGeneric CreateTask(CSharpType genericParamType)
    {
        return new CSharpTypeGeneric(TaskFullTypeName, [genericParamType]);
    }

    public static CSharpTypeVoid CreateVoid()
    {
        return new CSharpTypeVoid();
    }

    public static bool TryParse(string typeName, out CSharpType? type)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(typeName);

        var stack = new Stack<ScopeTracker>();
        var curScope = new ScopeTracker();

        for (int typeIndex = 0; typeIndex < typeName.Length; typeIndex++)
        {
            var curChar = typeName[typeIndex];
            if (curChar == '(')
            {
                if (curScope.DetectedType != "Unknown")
                {
                    type = null;
                    return false;
                }
                stack.Push(curScope);
                curScope.DetectedType = "Tuple";
                curScope = new ScopeTracker();
                curScope.EndingChar = ')';
            }
            else if (curChar == '<')
            {
                if (curScope.DetectedType != "Name")
                {
                    type = null;
                    return false;
                }
                stack.Push(curScope);
                curScope.DetectedType = "Generic";
                curScope = new ScopeTracker();
                curScope.EndingChar = '>';
            }
            else if (curChar == ',')
            {
                if (curScope.DetectedType == "Name")
                {
                    var scopeType = new CSharpTypeName(curScope.Buffer.ToString().Trim());
                    var prevScope = stack.Peek();
                    prevScope.Types.Add(scopeType);
                    curScope.Buffer.Clear();
                    curScope.DetectedType = "Unknown";
                }
            }
            else if (curChar == curScope.EndingChar)
            {
                var prevScope = stack.Pop();
                if (curScope.EndingChar == '>' && (curScope.DetectedType == "Unknown" || prevScope.DetectedType != "Generic"))
                {
                    type = null;
                    return false;
                }
                if (curScope.EndingChar == ')' && (curScope.DetectedType == "Unknown" || prevScope.DetectedType != "Tuple"))
                {
                    type = null;
                    return false;
                }

                if (curScope.DetectedType == "Name")
                {
                    var scopeType = new CSharpTypeName(curScope.Buffer.ToString().Trim());
                    curScope = prevScope;
                    curScope.Types.Add(scopeType);
                }
                else if (curScope.DetectedType == "Generic")
                {
                    var scopeType = new CSharpTypeGeneric(curScope.Buffer.ToString().Trim(), curScope.Types);
                    curScope = prevScope;
                    curScope.Types.Add(scopeType);
                }
                else if (curScope.DetectedType == "Tuple")
                {
                    var scopeType = new CSharpTypeTuple(curScope.Types.Select(s => new CSharpTupleElement(s)).ToList());
                    curScope = prevScope;
                    curScope.Types.Add(scopeType);
                }
            }
            else
            {
                if (curScope.DetectedType == "Unknown" && !char.IsWhiteSpace(curChar))
                {
                    curScope.DetectedType = "Name";
                }
                curScope.Buffer.Append(curChar);
            }
        }

        if (curScope.DetectedType == "Name")
        {
            type = new CSharpTypeName(curScope.Buffer.ToString().Trim());
            return true;
        }
        else if (curScope.DetectedType == "Generic")
        {
            type = new CSharpTypeGeneric(curScope.Buffer.ToString().Trim(), curScope.Types);
            return true;
        }
        else if (curScope.DetectedType == "Tuple")
        {
            type = new CSharpTypeTuple(curScope.Types.Select(s => new CSharpTupleElement(s)).ToList());
            return true;
        }

        type = null;
        return false;
    }

    private class ScopeTracker
    {
        public string DetectedType { get; set; } = "Unknown";
        public StringBuilder Buffer { get; } = new();
        public char? EndingChar { get; set; }
        public List<CSharpType> Types { get; } = new();
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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(typeName);
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
        ArgumentNullException.ThrowIfNullOrWhiteSpace(typeName);
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
    
    public static CSharpType WrapInTask(this CSharpType type)
    {
        if (type is CSharpTypeVoid)
        {
            return CSharpType.CreateTask();
        }

        return CSharpType.CreateTask(type);
    }
}