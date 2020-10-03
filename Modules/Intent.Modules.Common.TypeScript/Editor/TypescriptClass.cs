using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.TypeScript.Editor.Parsing;
using Zu.TypeScript;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptClass : TypeScriptNode
    {
        public TypeScriptClass(Node node, TypeScriptNode parent) : base(node, parent)
        {
        }

        public string Name => Node.IdentifierStr;
        public IList<TypeScriptMethod> Methods => Children.Where(x => x is TypeScriptMethod).Cast<TypeScriptMethod>().ToList();

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }

        public bool IsEmptyClass()
        {
            return Node.Last.Kind == SyntaxKind.Identifier && Node.Last.IdentifierStr == Name;
        }

        public override void UpdateNode(Node node)
        {
            base.UpdateNode(node);
            var walker = Editor.GetTreeWalker(this);
            walker.WalkTree();
        }
    }
}