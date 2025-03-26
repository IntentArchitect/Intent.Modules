#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Common.CSharp.Utils;

internal delegate string? TypeNameTransformer(string? typeName);

internal class GenericCSharpTypeParsingException : Exception
{
    public GenericCSharpTypeParsingException(string message) : base(message)
    {
    }

    public static GenericCSharpTypeParsingException InvalidCharacter(char character, int position)
    {
        return new GenericCSharpTypeParsingException($"Invalid character '{character}' found at position {position}.");
    }

    public static GenericCSharpTypeParsingException UnknownType(int position)
    {
        return new GenericCSharpTypeParsingException($"Could not determine type at position {position}.");
    }
}

internal class GenericCSharpTypeParser
{
    public static bool TryParse(string? typeName, TypeNameTransformer typeNameTransformer, out string? parsedOutput)
    {
        try
        {
            parsedOutput = Parse(typeName, typeNameTransformer);
            return true;
        }
        catch
        {
            parsedOutput = null;
            return false;
        }
    }
    
    public static string? Parse(string? typeName, TypeNameTransformer typeNameTransformer)
    {
        if (typeName is null)
        {
            return "void";
        }

        var parser = new GenericCSharpTypeParser(typeNameTransformer);
        for (parser._positionIndex = 0; parser._positionIndex < typeName.Length; parser._positionIndex++)
        {
            parser._currentChar = typeName[parser._positionIndex];
            parser.ProcessCharacter();
        }

        return parser.FinalizeScope();
    }

    private GenericCSharpTypeParser(TypeNameTransformer typeNameTransformer)
    {
        _typeNameTransformer = typeNameTransformer ?? throw new ArgumentNullException(nameof(typeNameTransformer));
        _scopeStack = new();
        _currentScope = new ScopeTracker();
    }

    private readonly TypeNameTransformer _typeNameTransformer;
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

    private void StartNewScope(DetectedType? expectedType, DetectedType newType, char endingChar)
    {
        if (expectedType is not null && _currentScope.DetectedType != expectedType)
        {
            throw GenericCSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
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
            throw GenericCSharpTypeParsingException.UnknownType(_positionIndex);
        }

        _currentScope.Modifiers.Add(Modifier.Nullable);
    }

    private void HandleScopeEnd()
    {
        var prevScope = _scopeStack.Pop();
        switch (_currentScope.EndingChar)
        {
            case '>' when _currentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Generic:
                // Helps to cater for Type<,> cases
                if (prevScope.DetectedType == DetectedType.Generic)
                {
                    _currentScope.DetectedType = DetectedType.Any;
                    goto default;
                }
                throw GenericCSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
            case ')' when _currentScope.DetectedType == DetectedType.Unknown || prevScope.DetectedType != DetectedType.Tuple:
                throw GenericCSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
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
        string? typeRepresentation;
        string? elementName = null;

        switch (_currentScope.DetectedType)
        {
            case DetectedType.Name:
            {
                var text = _currentScope.Buffer.ToString().Trim();
                string?[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                if (targetScope.DetectedType != DetectedType.Tuple && parts.Length > 1)
                {
                    throw GenericCSharpTypeParsingException.InvalidCharacter(_currentChar, _positionIndex);
                }

                if (parts[0] != "void")
                {
                    // Normalize the type name by removing namespaces
                    parts[0] = _typeNameTransformer(parts[0]);
                }

                typeRepresentation = parts[0];
                typeRepresentation = ApplyModifiers(typeRepresentation);
                elementName = parts.Skip(1).LastOrDefault();
            }
            break;
            
            case DetectedType.Generic:
            {
                // The content of Generic's buffer is just the inner types as strings
                var innerTypes = _currentScope.Entries.Select(s => s.Type).ToList();
                var baseName = _typeNameTransformer(_currentScope.Buffer.ToString().Trim());
                var delimiter = GetBestTypeDelimiter(innerTypes);
                typeRepresentation = $"{baseName}<{string.Join(delimiter, innerTypes)}>";
                typeRepresentation = ApplyModifiers(typeRepresentation);
            }
            break;
            
            case DetectedType.Tuple:
            {
                var tupleElements = _currentScope.Entries.Select(s => 
                    string.IsNullOrEmpty(s.ElementName) ? s.Type : $"{s.Type} {s.ElementName}").ToList();
                var delimiter = GetBestTypeDelimiter(tupleElements);
                typeRepresentation = $"({string.Join(delimiter, tupleElements)})";
                typeRepresentation = ApplyModifiers(typeRepresentation);
            }
            break;
            
            default:
                if (_scopeStack.Count != 0 && _scopeStack.Peek().DetectedType != DetectedType.Generic)
                {
                    throw GenericCSharpTypeParsingException.UnknownType(_positionIndex);
                }

                // Helps to cater for Type<,> cases
                typeRepresentation = "";
                _currentScope.DetectedType = DetectedType.Any;
                break;
        }

        targetScope.Entries.Add(new TypeEntry(typeRepresentation, elementName));
    }

    private string? FinalizeScope()
    {
        var text = _currentScope.Buffer.ToString().Trim();
        string? result;

        switch (_currentScope.DetectedType)
        {
            case DetectedType.Name:
                result = text == "void" ? "void" : _typeNameTransformer(text);
                break;

            case DetectedType.Generic:
            {
                var innerTypes = _currentScope.Entries.Select(e => e.Type).ToList();
                var baseName = _typeNameTransformer(text);
                var delimiter = GetBestTypeDelimiter(innerTypes);
                result = $"{baseName}<{string.Join(delimiter, innerTypes)}>";
            }
                break;

            case DetectedType.Tuple:
            {
                var tupleElements = _currentScope.Entries.Select(e =>
                    string.IsNullOrEmpty(e.ElementName) ? e.Type : $"{e.Type} {e.ElementName}").ToList();
                var delimiter = GetBestTypeDelimiter(tupleElements);
                result = $"({string.Join(delimiter, tupleElements)})";
            }
                break;

            default:
                throw GenericCSharpTypeParsingException.UnknownType(_positionIndex);
        }

        result = ApplyModifiers(result);
        return result;
    }

    private static string GetBestTypeDelimiter(List<string?>? items)
    {
        return items?.All(x => x is not null && x != "") == true ? ", " : ",";
    }

    private string? ApplyModifiers(string? type)
    {
        var modified = type;

        foreach (var modifier in _currentScope.Modifiers)
        {
            modified = modifier switch
            {
                Modifier.Nullable => $"{modified}?",
                Modifier.Array => $"{modified}[]",
                _ => throw new ArgumentOutOfRangeException($"Unsupported modifier: {modifier}")
            };
        }

        return modified;
    }

    private record TypeEntry(string? Type, string? ElementName = null);

    private class ScopeTracker
    {
        public DetectedType DetectedType { get; set; } = DetectedType.Unknown;
        public StringBuilder Buffer { get; } = new(32);
        public List<TypeEntry> Entries { get; } = [];
        public List<Modifier> Modifiers { get; } = [];
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
        Any, // Helps to cater for Type<,> cases
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