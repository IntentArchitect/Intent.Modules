#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp;

internal class TypeRegistry
{
    private readonly TypePathPart _globalNamespace = new(isType: false, parent: null);

    public TypeRegistry WithTypes(IEnumerable<string> fullyQualifiedTypeNames)
    {
        foreach (var fullyQualifiedTypeName in fullyQualifiedTypeNames)
        {
            Add(fullyQualifiedTypeName);
        }

        return this;
    }

    public TypeRegistry WithNamespaces(IEnumerable<string> namespaces)
    {
        foreach (var @namespace in namespaces)
        {
            Add(@namespace, typeName: null);
        }

        return this;
    }

    public void Add(string fullyQualifiedTypeName)
    {
        var split = fullyQualifiedTypeName.Split('.');
        Add(split[..^1], split[^1..]);
    }

    public void Add(string @namespace, string? typeName)
    {
        Add(@namespace.Split('.'), typeName == null ? [] : [typeName]);
    }

    public void Add(IEnumerable<string> namespaceParts, IEnumerable<string> typeNameParts)
    {
        var current = _globalNamespace;

        foreach (var part in namespaceParts)
        {
            if (current.ContainsKey(part))
            {
                current = current[part];
            }
            else
            {
                var typePartPath = new TypePathPart(isType: false, parent: current);
                current[part] = typePartPath;
                current = typePartPath;
            }
        }

        foreach (var part in typeNameParts)
        {
            if (current.ContainsKey(part))
            {
                current = current[part];
                current.IsType = true;
            }
            else
            {
                var typePartPath = new TypePathPart(isType: true, parent: current);
                current[part] = typePartPath;
                current = typePartPath;
            }
        }
    }

    public bool Contains(string path, out bool isType)
    {
        return Contains(path.Split('.'), null, out isType, out _);
    }

    public bool Contains(string @namespace, string typeName, out bool isType)
    {
        return Contains(@namespace.Split('.'), typeName, out isType, out _);
    }

    public bool Contains(Span<string> typeParts, string? typeName, out bool isType, out bool isNested)
    {
        var current = _globalNamespace;

        foreach (var part in typeParts)
        {
            if (!current.TryGetValue(part, out current))
            {
                isType = default;
                isNested = default;
                return false;
            }
        }

        if (typeName != null && !current.TryGetValue(typeName, out current))
        {
            isType = default;
            isNested = default;
            return false;
        }

        isType = current.IsType;
        isNested = current.IsNested;
        return true;
    }

    public bool IsNestedType(Span<string> typeParts, string? typeName)
    {
        return Contains(typeParts, typeName, out _, out var isNested) && isNested;
    }

    private class TypePathPart(bool isType, TypePathPart? parent) : Dictionary<string, TypePathPart>
    {
        private bool _isType = isType;

        public bool IsNested => parent?.IsType == true;

        public bool IsType
        {
            get => _isType || IsNested;
            set => _isType = value;
        }
    }
}