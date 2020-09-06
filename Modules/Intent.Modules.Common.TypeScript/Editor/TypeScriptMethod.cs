using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptMethod : TypeScriptNode
    {
        public TypeScriptMethod(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public override string GetIdentifier(Node node)
        {
            return Node.IdentifierStr;
        }

        //public string Name => Node.IdentifierStr;

        //public bool Equals(TypeScriptMethod other)
        //{
        //    return Name == other?.Name;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((TypeScriptMethod) obj);
        //}

        //public override int GetHashCode()
        //{
        //    return Name.GetHashCode();
        //}

    }
}