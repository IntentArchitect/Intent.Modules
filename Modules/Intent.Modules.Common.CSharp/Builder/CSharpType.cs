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

        var scopeStack = new Stack<ScopeTracker>();
        var curScope = new ScopeTracker();

        for (var typeIndex = 0; typeIndex < typeName.Length; typeIndex++)
        {
            var curChar = typeName[typeIndex];
            if (curChar == '(')
            {
                if (curScope.DetectedType != DetectedType.Unknown)
                {
                    type = null;
                    return false;
                }

                scopeStack.Push(curScope);
                curScope.DetectedType = DetectedType.Tuple;
                curScope = new ScopeTracker();
                curScope.EndingChar = ')';
            }
            else if (curChar == '<')
            {
                if (curScope.DetectedType != DetectedType.Name)
                {
                    type = null;
                    return false;
                }

                scopeStack.Push(curScope);
                curScope.DetectedType = DetectedType.Generic;
                curScope = new ScopeTracker();
                curScope.EndingChar = '>';
            }
            else if (curChar == ',')
            {
                if (curScope.DetectedType == DetectedType.Name)
                {
                    var prevScope = scopeStack.Peek();
                    var text = curScope.Buffer.ToString().Trim();
                    var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (prevScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                    {
                        type = null;
                        return false;
                    }
                    var scopeType = new CSharpTypeName(parts[0]);
                    prevScope.Entries.Add(new TypeEntry(scopeType, parts.Skip(1).LastOrDefault()));
                    curScope.Reset();
                }
            }
            else if (curChar == curScope.EndingChar)
            {
                var prevScope = scopeStack.Pop();
                switch (curScope.EndingChar)
                {
                    case '>' when curScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Generic:
                        type = null;
                        return false;
                    case ')' when curScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Tuple:
                        type = null;
                        return false;
                }

                switch (curScope.DetectedType)
                {
                    case DetectedType.Name:
                    {
                        var text = curScope.Buffer.ToString().Trim();
                        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (prevScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                        {
                            type = null;
                            return false;
                        }
                        var scopeType = new CSharpTypeName(parts[0]);
                        curScope = prevScope;
                        curScope.Entries.Add(new TypeEntry(scopeType, parts.Skip(1).LastOrDefault()));
                        break;
                    }
                    case DetectedType.Generic:
                    {
                        var scopeType = new CSharpTypeGeneric(curScope.Buffer.ToString().Trim(), curScope.Entries.Select(s => s.Type).ToList());
                        curScope = prevScope;
                        curScope.Entries.Add(new TypeEntry(scopeType));
                        break;
                    }
                    case DetectedType.Tuple:
                    {
                        var scopeType = new CSharpTypeTuple(curScope.Entries.Select(s => new CSharpTupleElement(s.Type, s.Name)).ToList());
                        curScope = prevScope;
                        curScope.Entries.Add(new TypeEntry(scopeType));
                        break;
                    }
                }
            }
            else
            {
                // Whitespace should not count as an identifier
                if (curScope.DetectedType == DetectedType.Unknown && !char.IsWhiteSpace(curChar))
                {
                    curScope.DetectedType = DetectedType.Name;
                }

                curScope.Buffer.Append(curChar);
            }
        }

        switch (curScope.DetectedType)
        {
            case DetectedType.Name:
                type = new CSharpTypeName(curScope.Buffer.ToString().Trim());
                return true;
            case DetectedType.Generic:
                type = new CSharpTypeGeneric(curScope.Buffer.ToString().Trim(), curScope.Entries.Select(s => s.Type).ToList());
                return true;
            case DetectedType.Tuple:
                type = new CSharpTypeTuple(curScope.Entries.Select(s => new CSharpTupleElement(s.Type, s.Name)).ToList());
                return true;
            default:
                type = null;
                return false;
        }
    }

    private record TypeEntry(CSharpType Type, string? Name = null);
    private class ScopeTracker
    {
        public DetectedType DetectedType { get; set; } = DetectedType.Unknown;
        public StringBuilder Buffer { get; } = new();
        public List<TypeEntry> Entries { get; } = new();
        public char? EndingChar { get; set; }

        public void Reset()
        {
            Buffer.Clear();
            Entries.Clear();
            DetectedType = DetectedType.Unknown;
        }
    }

    private enum DetectedType
    {
        Unknown,
        Name,
        Generic,
        Tuple
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