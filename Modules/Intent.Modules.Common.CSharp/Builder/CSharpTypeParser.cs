using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace Intent.Modules.Common.CSharp.Builder;

public static class CSharpTypeParser
{
    public static bool TryParse(string typeName, out CSharpType? type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

        var scopeStack = new Stack<ScopeTracker>();
        var currentScope = new ScopeTracker();

        for (var index = 0; index < typeName.Length; index++)
        {
            var currentChar = typeName[index];
            if (!ProcessCharacter(currentChar, ref currentScope, scopeStack))
            {
                type = null;
                return false;
            }
        }

        return FinalizeScope(currentScope, out type);
    }

    private static bool ProcessCharacter(char currentChar, ref ScopeTracker currentScope, Stack<ScopeTracker> scopeStack)
    {
        switch (currentChar)
        {
            case '(':
                return StartNewScope(ref currentScope, scopeStack, DetectedType.Unknown, DetectedType.Tuple, ')');
            case '<':
                return StartNewScope(ref currentScope, scopeStack, DetectedType.Name, DetectedType.Generic, '>');
            case ',':
                return HandleComma(ref currentScope, scopeStack);
            case var _ when currentScope.EndingChar == currentChar:
                return HandleScopeEnd(ref currentScope, scopeStack);
            default:
                AppendCharToScope(ref currentScope, currentChar);
                return true;
        }
    }

    private static bool StartNewScope(ref ScopeTracker currentScope, Stack<ScopeTracker> scopeStack, DetectedType expectedType, DetectedType newType, char endingChar)
    {
        if (currentScope.DetectedType != expectedType)
        {
            return false;
        }

        currentScope.DetectedType = newType;
        scopeStack.Push(currentScope);
        
        currentScope = new ScopeTracker { EndingChar = endingChar };
        return true;
    }

    private static bool HandleComma(ref ScopeTracker currentScope, Stack<ScopeTracker> scopeStack)
    {
        var prevScope = scopeStack.Peek();
        if (!FinalizeSubScope(currentScope, prevScope))
        {
            return false;
        }

        currentScope.Reset();
        return true;
    }

    private static bool HandleScopeEnd(ref ScopeTracker currentScope, Stack<ScopeTracker> scopeStack)
    {
        var prevScope = scopeStack.Pop();
        switch (currentScope.EndingChar)
        {
            case '>' when currentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Generic:
                return false;
            case ')' when currentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Tuple:
                return false;
        }
        
        if (!FinalizeSubScope(currentScope, prevScope))
        {
            return false;
        }

        currentScope = prevScope;
        return true;
    }
    
    private static bool FinalizeSubScope(ScopeTracker currentScope, ScopeTracker prevScope)
    {
        switch (currentScope.DetectedType)
        {
            case DetectedType.Name:
            {
                var text = currentScope.Buffer.ToString().Trim();
                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (prevScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                {
                    return false;
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
                    new CSharpTypeTuple(currentScope.Entries.Select(s => new CSharpTupleElement(s.Type, s.Name)).ToList())));
                break;
            default:
                return false;
        }

        return true;
    }

    private static void AppendCharToScope(ref ScopeTracker currentScope, char currentChar)
    {
        // Whitespace should not count as an identifier
        if (currentScope.DetectedType == DetectedType.Unknown && !char.IsWhiteSpace(currentChar))
        {
            currentScope.DetectedType = DetectedType.Name;
        }

        currentScope.Buffer.Append(currentChar);
    }

    private static bool FinalizeScope(ScopeTracker currentScope, out CSharpType? type)
    {
        switch (currentScope.DetectedType)
        {
            case DetectedType.Name:
                type = new CSharpTypeName(currentScope.Buffer.ToString().Trim());
                return true;
            case DetectedType.Generic:
                type = new CSharpTypeGeneric(currentScope.Buffer.ToString().Trim(), currentScope.Entries.Select(e => e.Type).ToList());
                return true;
            case DetectedType.Tuple:
                type = new CSharpTypeTuple(currentScope.Entries.Select(e => new CSharpTupleElement(e.Type, e.Name)).ToList());
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