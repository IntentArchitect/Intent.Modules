using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptDecorator : TypeScriptNode, IEquatable<TypeScriptDecorator>
    {
        public TypeScriptDecorator(Node node, TypeScriptFile file) : base(node, file)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Decorator name could not be determined");
            }
        }

        public string Name => Node.First.IdentifierStr;

        public IEnumerable<TypeScriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypeScriptObjectLiteralExpression(x, File));
        }

        internal override void UpdateNode()
        {
            Node = FindNodes(File.Ast.RootNode, NodePath).First(x => x.OfKind(SyntaxKind.Identifier).First().IdentifierStr == Name);
        }

        public override bool IsIgnored() => false;

        public bool Equals(TypeScriptDecorator other)
        {
            return Name == other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptDecorator)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}