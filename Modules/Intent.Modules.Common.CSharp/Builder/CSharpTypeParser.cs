using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

/// <summary>
/// Use this class to parse a string and convert it into an appropriate <see cref="CSharpType"/>.
/// </summary>
public class CSharpTypeParser
{
    /// <summary>
    /// Parses a string that represents a C# type and attempts to produce a <see cref="CSharpType"/> tree type. 
    /// </summary>
    /// <param name="typeName">The C# type in string form. If null is supplied, it will return with <see cref="CSharpTypeVoid" />.</param>
    /// <param name="type">Provides the type instance if the parsing was done successful.</param>
    /// <returns>True if the parsing was successful otherwise false.</returns>
    public static bool TryParse(string? typeName, out CSharpType? type)
    {
        try
        {
            type = Parse(typeName);
            return true;
        }
        catch (Exception)
        {
            type = null;
            return false;
        }
    }

    /// <summary>
    /// Parses a string that represents a C# type and attempts to produce a <see cref="CSharpType"/> tree type. 
    /// </summary>
    /// <param name="typeName">The C# type in string form. If null is supplied, it will return with <see cref="CSharpTypeVoid" />.</param>
    public static CSharpType Parse(string? typeName)
    {
        if (typeName is null)
        {
            return CSharpType.CreateVoid();
        }

        var parser = new CSharpTypeParser();
        for (parser._positionIndex = 0; parser._positionIndex < typeName.Length; parser._positionIndex++)
        {
            parser._currentChar = typeName[parser._positionIndex];
            parser.ProcessCharacter();
        }

        return parser.FinalizeScope();
    }

    private CSharpTypeParser()
    {
        _scopeStack = new();
        _currentScope = new ScopeTracker();
    }

    private readonly Stack<ScopeTracker> _scopeStack;
    private ScopeTracker _currentScope;
    private char _currentChar;
    private int _positionIndex;
    
    
    private void ProcessCharacter()
    {
        switch (_currentChar)
        {
            case '(':
                StartNewScope(expectedType: DetectedType.Unknown, newType: DetectedType.Tuple, endingChar: ')');
                break;
            case '<':
                StartNewScope(expectedType: DetectedType.Name, newType: DetectedType.Generic, endingChar: '>');
                break;
            case '[':
                StartNewScope(expectedType: null, newType: _currentScope.DetectedType, endingChar: ']');
                break;
            case ',':
                HandleComma();
                break;
            case '?':
                HandleNullSymbol();
                break;
            case var _ when _currentScope.EndingChar == _currentChar:
                HandleScopeEnd();
                break;
            default:
                AppendCharToScope();
                break;
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private void StartNewScope(DetectedType? expectedType, DetectedType newType, char endingChar)
    {
        if (expectedType is not null && _currentScope.DetectedType != expectedType)
        {
            throw CSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
        }

        _currentScope.DetectedType = newType;
        _scopeStack.Push(_currentScope);

        _currentScope = new ScopeTracker { EndingChar = endingChar };
    }

    private void HandleComma()
    {
        FinalizeSubScope(_scopeStack.Peek());
        _currentScope.Reset();
    }
    
    private void HandleNullSymbol()
    {
        if (_currentScope.DetectedType is DetectedType.Unknown)
        {
            throw CSharpTypeParsingException.UnknownType(_positionIndex);
        }

        _currentScope.Modifiers.Add(Modifier.Nullable);
    }

    private void HandleScopeEnd()
    {
        var prevScope = _scopeStack.Pop();
        switch (_currentScope.EndingChar)
        {
            case '>' when _currentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Generic:
                throw CSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
            case ')' when _currentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Tuple:
                throw CSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
            case ']':
                prevScope.Modifiers.Add(Modifier.Array);
                _currentScope = prevScope;
                return;
            default:
                FinalizeSubScope(prevScope);
                _currentScope = prevScope;
                return;
        }
    }

    private void AppendCharToScope()
    {
        // Whitespace should not count as an identifier
        if (_currentScope.DetectedType == DetectedType.Unknown && !char.IsWhiteSpace(_currentChar))
        {
            _currentScope.DetectedType = DetectedType.Name;
        }

        _currentScope.Buffer.Append(_currentChar);
    }

    private void FinalizeSubScope(ScopeTracker targetScope)
    {
        TypeEntry typeEntry;
        switch (_currentScope.DetectedType)
        {
            case DetectedType.Name:
            {
                var text = _currentScope.Buffer.ToString().Trim();
                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (targetScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                {
                    throw CSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
                }

                CSharpType scopeType = parts[0] == "void" ? new CSharpTypeVoid() : new CSharpTypeName(parts[0]);
                scopeType = ApplyModifiers(scopeType);
                typeEntry = new TypeEntry(Type: scopeType, ElementName: parts.Skip(1).LastOrDefault());
            }
                break;
            case DetectedType.Generic:
            {
                CSharpType scopeType = new CSharpTypeGeneric(_currentScope.Buffer.ToString().Trim(), _currentScope.Entries.Select(s => s.Type).ToList());
                scopeType = ApplyModifiers(scopeType);
                typeEntry = new TypeEntry(scopeType);
            }
                break;
            case DetectedType.Tuple:
            {
                CSharpType scopeType = new CSharpTypeTuple(_currentScope.Entries.Select(s => new CSharpTupleElement(s.Type, s.ElementName)).ToList());
                scopeType = ApplyModifiers(scopeType);
                typeEntry = new TypeEntry(scopeType);
            }
                break;
            default:
                throw CSharpTypeParsingException.UnknownType(_positionIndex);
        }

        targetScope.Entries.Add(typeEntry);
    }

    private CSharpType FinalizeScope()
    {
        var text = _currentScope.Buffer.ToString().Trim();
        if (text == "void")
        {
            return new CSharpTypeVoid();
        }

        CSharpType type;
        switch (_currentScope.DetectedType)
        {
            case DetectedType.Name:
                type = new CSharpTypeName(text);
                break;
            case DetectedType.Generic:
                type =  new CSharpTypeGeneric(text, _currentScope.Entries.Select(e => e.Type).ToList());
                break;
            case DetectedType.Tuple:
                type =  new CSharpTypeTuple(_currentScope.Entries.Select(e => new CSharpTupleElement(e.Type, e.ElementName)).ToList());
                break;
            default:
                throw CSharpTypeParsingException.UnknownType(_positionIndex);
        }

        type = ApplyModifiers(type);

        return type;
    }

    private CSharpType ApplyModifiers(CSharpType type)
    {
        CSharpType modified = type;
        
        foreach (var modifier in _currentScope.Modifiers)
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