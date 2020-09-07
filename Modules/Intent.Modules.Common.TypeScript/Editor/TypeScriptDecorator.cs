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
            return Node.First.IdentifierStr;
        }

        public IEnumerable<TypeScriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypeScriptObjectLiteralExpression(x, Editor));
        }

        //internal override void UpdateNode()
        //{
        //    Node = _parent.Node.OfKind(SyntaxKind.Decorator).First(x => x.OfKind(SyntaxKind.Identifier).First().IdentifierStr == Name);
        //    //Node = FindNodes(File.Ast.RootNode, NodePath).First(x => x.OfKind(SyntaxKind.Identifier).First().IdentifierStr == Name);
        //}


        public override bool IsIgnored() => false;

        //public bool Equals(TypeScriptDecorator other)
        //{
        //    return Name == other?.Name;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((TypeScriptDecorator)obj);
        //}

        //public override int GetHashCode()
        //{
        //    return Name.GetHashCode();
        //}
    }
}