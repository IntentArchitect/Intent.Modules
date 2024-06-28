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
            throw new CSharpTypeParsingException($"Invalid character '{CurrentChar}' found at position {PositionIndex}.");
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
                throw new CSharpTypeParsingException($"Invalid character '{CurrentChar}' found at position {PositionIndex}.");
            case ')' when CurrentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Tuple:
                throw new CSharpTypeParsingException($"Invalid character '{CurrentChar}' found at position {PositionIndex}.");
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
                    throw new CSharpTypeParsingException($"Invalid character '{CurrentChar}' found at position {PositionIndex}.");
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
                throw new CSharpTypeParsingException($"Could not infer type at position {PositionIndex}.");
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
                throw new CSharpTypeParsingException($"Could not infer type at position {PositionIndex}.");
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