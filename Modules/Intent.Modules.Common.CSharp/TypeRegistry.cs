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
            for (int i  = 0; i < split.Length; i++)
            {
                var item = split[i];
                if (current.ContainsKey(item))
                {
                    current = current[item];
                }
                else
                {
                    current[item] = current = new Member( i == split.Length - 1);
                }
            }
        }
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
                current[item] = current = new Member(false);
            }
        }
        current[typeName] = new Member(true);
    }

    public bool Contains(string @namespace)
    {
        var current = _members;
        
        var split = @namespace.Split('.');
        foreach (var item in split)
        {
            if (!current.TryGetValue(item, out current))
            {
                return false;
            }
        }

        return true;
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

    private class Member : Dictionary<string, Member> 
    {
        public bool IsType { get; }
        public Member(bool isType = false)
        {                
            IsType = isType;
        }
    }
}