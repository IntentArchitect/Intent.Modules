using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public class CSharpTypeParsingException : Exception
{
    public CSharpTypeParsingException(string message) : base(message)
    {
    }

    public static CSharpTypeParsingException InvalidCharacter(char character, int position)
    {
        return new CSharpTypeParsingException($"Invalid character '{character}' found at position {position}.");
    }

    public static CSharpTypeParsingException UnknownType(int postition)
    {
        return new CSharpTypeParsingException($"Could not determine type at position {postition}.");
    }
}

public class CSharpTypeParser
{
    public static bool TryParse(string typeName, out CSharpType? type)
    {
        try
        {
            type = Parse(typeName);
            return true;
        }
        catch (Exception exception)
        {
            type = null;
            return false;
        }
    }

    public static CSharpType Parse(string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

        var parser = new CSharpTypeParser();
        for (parser.PositionIndex = 0; parser.PositionIndex < typeName.Length; parser.PositionIndex++)
        {
            parser.CurrentChar = typeName[parser.PositionIndex];
            parser.ProcessCharacter();
        }

        return parser.FinalizeScope();
    }

    private CSharpTypeParser()
    {
        ScopeStack = new();
        CurrentScope = new ScopeTracker();
    }

    private readonly Stack<ScopeTracker> ScopeStack;
    private ScopeTracker CurrentScope;
    private char CurrentChar;
    private int PositionIndex;
    
    
    private void ProcessCharacter()
    {
        switch (CurrentChar)
        {
            case '(':
                StartNewScope(expectedType: DetectedType.Unknown, newType: DetectedType.Tuple, endingChar: ')');
                break;
            case '<':
                StartNewScope(expectedType: DetectedType.Name, newType: DetectedType.Generic, endingChar: '>');
                break;
            case '[':
                StartNewScope(expectedType: null, newType: CurrentScope.DetectedType, endingChar: ']');
                break;
            case ',':
                HandleComma();
                break;
            case '?':
                HandleNullSymbol();
                break;
            case var _ when CurrentScope.EndingChar == CurrentChar:
                HandleScopeEnd();
                break;
            default:
                AppendCharToScope();
                break;
        }
    }

    private void StartNewScope(DetectedType? expectedType, DetectedType newType, char endingChar)
    {
        if (expectedType is not null && CurrentScope.DetectedType != expectedType)
        {
            throw CSharpTypeParsingException.InvalidCharacter(CurrentChar, PositionIndex);
        }

        CurrentScope.DetectedType = newType;
        ScopeStack.Push(CurrentScope);

        CurrentScope = new ScopeTracker { EndingChar = endingChar };
    }

    private void HandleComma()
    {
        FinalizeSubScope(ScopeStack.Peek());
        CurrentScope.Reset();
    }
    
    private void HandleNullSymbol()
    {
        if (CurrentScope.DetectedType is DetectedType.Unknown)
        {
            throw CSharpTypeParsingException.UnknownType(PositionIndex);
        }

        CurrentScope.Modifiers.Add(Modifier.Nullable);
    }

    private void HandleScopeEnd()
    {
        var prevScope = ScopeStack.Pop();
        switch (CurrentScope.EndingChar)
        {
            case '>' when CurrentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Generic:
                throw CSharpTypeParsingException.InvalidCharacter(CurrentChar, PositionIndex);
            case ')' when CurrentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Tuple:
                throw CSharpTypeParsingException.InvalidCharacter(CurrentChar, PositionIndex);
            case ']':
                prevScope.Modifiers.Add(Modifier.Array);
                CurrentScope = prevScope;
                return;
            default:
                FinalizeSubScope(prevScope);
                CurrentScope = prevScope;
                return;
        }
    }

    private void AppendCharToScope()
    {
        // Whitespace should not count as an identifier
        if (CurrentScope.DetectedType == DetectedType.Unknown && !char.IsWhiteSpace(CurrentChar))
        {
            CurrentScope.DetectedType = DetectedType.Name;
        }

        CurrentScope.Buffer.Append(CurrentChar);
    }

    private void FinalizeSubScope(ScopeTracker targetScope)
    {
        TypeEntry typeEntry;
        switch (CurrentScope.DetectedType)
        {
            case DetectedType.Name:
            {
                var text = CurrentScope.Buffer.ToString().Trim();
                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (targetScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                {
                    throw CSharpTypeParsingException.InvalidCharacter(CurrentChar, PositionIndex);
                }

                CSharpType scopeType = parts[0] == "void" ? new CSharpTypeVoid() : new CSharpTypeName(parts[0]);
                scopeType = ApplyModifiers(scopeType);
                typeEntry = new TypeEntry(Type: scopeType, ElementName: parts.Skip(1).LastOrDefault());
            }
                break;
            case DetectedType.Generic:
            {
                CSharpType scopeType = new CSharpTypeGeneric(CurrentScope.Buffer.ToString().Trim(), CurrentScope.Entries.Select(s => s.Type).ToList());
                scopeType = ApplyModifiers(scopeType);
                typeEntry = new TypeEntry(scopeType);
            }
                break;
            case DetectedType.Tuple:
            {
                CSharpType scopeType = new CSharpTypeTuple(CurrentScope.Entries.Select(s => new CSharpTupleElement(s.Type, s.ElementName)).ToList());
                scopeType = ApplyModifiers(scopeType);
                typeEntry = new TypeEntry(scopeType);
            }
                break;
            default:
                throw CSharpTypeParsingException.UnknownType(PositionIndex);
        }

        targetScope.Entries.Add(typeEntry);
    }

    private CSharpType FinalizeScope()
    {
        var text = CurrentScope.Buffer.ToString().Trim();
        if (text == "void")
        {
            return new CSharpTypeVoid();
        }

        CSharpType type;
        switch (CurrentScope.DetectedType)
        {
            case DetectedType.Name:
                type = new CSharpTypeName(text);
                break;
            case DetectedType.Generic:
                type =  new CSharpTypeGeneric(text, CurrentScope.Entries.Select(e => e.Type).ToList());
                break;
            case DetectedType.Tuple:
                type =  new CSharpTypeTuple(CurrentScope.Entries.Select(e => new CSharpTupleElement(e.Type, e.ElementName)).ToList());
                break;
            default:
                throw CSharpTypeParsingException.UnknownType(PositionIndex);
        }

        type = ApplyModifiers(type);

        return type;
    }

    private CSharpType ApplyModifiers(CSharpType type)
    {
        CSharpType modified = type;
        
        foreach (var modifier in CurrentScope.Modifiers)
        {
            switch (modifier)
            {
                case Modifier.Nullable:
                    modified = new CSharpTypeNullable(modified);
                    break;
                case Modifier.Array:
                    modified = new CSharpTypeArray(modified);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return modified;
    }
    
    private record TypeEntry(CSharpType Type, string? ElementName = null);

    private class ScopeTracker
    {
        public DetectedType DetectedType { get; set; } = DetectedType.Unknown;
        public StringBuilder Buffer { get; } = new();
        public List<TypeEntry> Entries { get; } = new();
        public List<Modifier> Modifiers { get; } = new();
        public char? EndingChar { get; set; }

        public void Reset()
        {
            Buffer.Clear();
            Entries.Clear();
            Modifiers.Clear();
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

    private enum Modifier
    {
        Nullable,
        Array
    }
}