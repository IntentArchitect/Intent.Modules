using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptDecorator : TypeScriptNode
    {
        private readonly TypeScriptNode _parent;

        public TypeScriptDecorator(Node node, TypeScriptNode parent) : base(node, parent.Editor)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Decorator name could not be determined");
            }

            _parent = parent;
        }

        public string Name => Node.First.IdentifierStr;

        public override string GetIdentifier(Node node)
        {
            return node.First.IdentifierStr;
        }

        public IEnumerable<TypeScriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypeScriptObjectLiteralExpression(x, Editor));
        }

        public override bool IsIgnored()
        {
            return _parent.IsIgnored();
        }

        public override bool IsMerged()
        {
            return _parent.IsMerged();
        }
    }
}