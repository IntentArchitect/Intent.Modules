using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.CSharp;

internal class TypeRegistry
{
    private readonly Member _members = new();

    public TypeRegistry(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            var current = _members;

            var split = path.Split('.');
            for (var i = 0; i < split.Length; i++)
            {
                var item = split[i];
                if (current.ContainsKey(item))
                {
                    current = current[item];
                }
                else
                {
                    var member = new Member(i == split.Length - 1);
                    current[item] = member;
                    current = member;
                }
            }
        }
    }

    public void Add(string fullyQualifiedTypeName)
    {
        var lastIndexOf = fullyQualifiedTypeName.LastIndexOf('.');
        Add(fullyQualifiedTypeName[..lastIndexOf], fullyQualifiedTypeName[(lastIndexOf + 1)..]);
    }

    public void Add(string @namespace, string typeName)
    {
        var current = _members;

        var split = @namespace.Split('.');
        foreach (var item in split)
        {
            if (current.ContainsKey(item))
            {
                current = current[item];
            }
            else
            {
                current = new Member();
                current[item] = current;
            }
        }
        current[typeName] = new Member(true);
    }

    public bool Contains(string @namespace)
    {
        var current = _members;

        var split = @namespace.Split('.');
        return split.All(item => current.TryGetValue(item, out current));
    }

    public bool ContainsType(string typeName)
    {
        var current = _members;

        var split = typeName.Split('.');
        foreach (var item in split)
        {
            if (!current.TryGetValue(item, out current))
            {
                return false;
            }
        }

        return current.IsType;
    }


    public bool Contains(string @namespace, string typeUnqualified)
    {
        var current = _members;

        var split = @namespace.Split('.');
        foreach (var item in split.Append(typeUnqualified))
        {
            if (!current.TryGetValue(item, out current))
            {
                return false;
            }
        }

        return current.Count == 0;
    }

    private class Member(bool isType = false) : Dictionary<string, Member>
    {
        public bool IsType { get; } = isType;
    }
}