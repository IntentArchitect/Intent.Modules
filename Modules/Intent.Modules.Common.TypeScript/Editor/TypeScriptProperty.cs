using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{

    public class TypeScriptGetAccessor : TypeScriptNode
    {
        public TypeScriptGetAccessor(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {

        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }
    }

    public class TypeScriptSetAccessor : TypeScriptNode
    {
        public TypeScriptSetAccessor(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {

        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }
    }

    public class TypeScriptProperty : TypeScriptNode
    {
        public TypeScriptProperty(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {

        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }
    }
}