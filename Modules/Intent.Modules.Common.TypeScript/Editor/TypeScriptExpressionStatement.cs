using System;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptExpressionStatement : TypeScriptNode, IEquatable<TypeScriptExpressionStatement>
    {
        public TypeScriptExpressionStatement(Node node, TypeScriptFile file) : base(node, file)
        {
            Identifier = Node.OfKind(SyntaxKind.PropertyAccessExpression).First().GetText() ?? throw new ArgumentException("Variable identifier could not be determined for node: " + this);
            NodePath += $"/PropertyAccessExpression~{Identifier}";
        }

        public string Identifier { get; }

        public T GetAssignedValue<T>()
            where T : TypeScriptNode
        {
            var arrayLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ArrayLiteralExpression);
            if (arrayLiteral != null)
            {
                return new TypeScriptArrayLiteralExpression(arrayLiteral, File) as T;
            }
            var objectLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ObjectLiteralExpression);
            if (objectLiteral != null)
            {
                return new TypeScriptObjectLiteralExpression(objectLiteral, File) as T;
            }
            // TODO: ValueLiteral

            return null;
        }

        internal override void UpdateNode()
        {
            Node = (Node)FindNode(File.Ast.RootNode, NodePath).Parent.Parent;
        }

        public bool Equals(TypeScriptExpressionStatement other)
        {
            return Identifier == other?.Identifier;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptExpressionStatement)obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}