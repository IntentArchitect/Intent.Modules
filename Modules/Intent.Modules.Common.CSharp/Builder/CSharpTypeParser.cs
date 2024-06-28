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
                StartNewScope(DetectedType.Unknown, DetectedType.Tuple, ')');
                break;
            case '<':
                StartNewScope(DetectedType.Name, DetectedType.Generic, '>');
                break;
            case ',':
                HandleComma();
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
        FinalizeSubScope(CurrentScope, ScopeStack.Peek());
        CurrentScope.Reset();
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
        }

        FinalizeSubScope(CurrentScope, prevScope);

        CurrentScope = prevScope;
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

    private void FinalizeSubScope(ScopeTracker currentScope, ScopeTracker prevScope)
    {
        switch (currentScope.DetectedType)
        {
            case DetectedType.Name:
            {
                var text = currentScope.Buffer.ToString().Trim();
                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (prevScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                {
                    throw CSharpTypeParsingException.InvalidCharacter(CurrentChar, PositionIndex);
                }

                var scopeType = new CSharpTypeName(parts[0]);
                prevScope.Entries.Add(new TypeEntry(scopeType, parts.Skip(1).LastOrDefault()));
            }
                break;
            case DetectedType.Generic:
                prevScope.Entries.Add(new TypeEntry(
                    new CSharpTypeGeneric(currentScope.Buffer.ToString().Trim(), currentScope.Entries.Select(s => s.Type).ToList())));
                break;
            case DetectedType.Tuple:
                prevScope.Entries.Add(new TypeEntry(
                    new CSharpTypeTuple(currentScope.Entries.Select(s => new CSharpTupleElement(s.Type, s.ElementName)).ToList())));
                break;
            default:
                throw CSharpTypeParsingException.UnknownType(PositionIndex);
        }
    }

    private CSharpType FinalizeScope()
    {
        switch (CurrentScope.DetectedType)
        {
            case DetectedType.Name:
                return new CSharpTypeName(CurrentScope.Buffer.ToString().Trim());
            case DetectedType.Generic:
                return new CSharpTypeGeneric(CurrentScope.Buffer.ToString().Trim(), CurrentScope.Entries.Select(e => e.Type).ToList());
            case DetectedType.Tuple:
                return new CSharpTypeTuple(CurrentScope.Entries.Select(e => new CSharpTupleElement(e.Type, e.ElementName)).ToList());
            default:
                throw CSharpTypeParsingException.UnknownType(PositionIndex);
        }
    }
    
    private record TypeEntry(CSharpType Type, string? ElementName = null);

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