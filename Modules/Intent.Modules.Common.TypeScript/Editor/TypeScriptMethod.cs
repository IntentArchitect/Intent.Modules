using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptMethod : TypeScriptNode
    {
        public TypeScriptMethod(Node node, TypeScriptNode parent) : base(node, parent)
        {

        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }
    }
}