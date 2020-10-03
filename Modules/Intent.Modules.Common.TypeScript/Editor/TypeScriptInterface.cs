using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.TypeScript.Editor.Parsing;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptInterface : TypeScriptNode
    {
        public TypeScriptInterface(Node node, TypeScriptNode parent) : base(node, parent)
        {
        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }

        public override void UpdateNode(Node node)
        {
            base.UpdateNode(node);
            var walker = Editor.GetTreeWalker(this);
            walker.WalkTree();
        }
    }
}