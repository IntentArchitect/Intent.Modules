using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptConstructor : TypeScriptNode
    {
        public TypeScriptConstructor(Node node, TypeScriptNode parent) : base(node, parent)
        {
        }

        public override string GetIdentifier(Node node)
        {
            return null;
        }
    }
}