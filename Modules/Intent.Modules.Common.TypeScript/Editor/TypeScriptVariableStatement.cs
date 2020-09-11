using System;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptVariableStatement : TypeScriptNode
    {
        public TypeScriptVariableStatement(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
            //Name = Node.OfKind(SyntaxKind.VariableDeclaration).First().IdentifierStr ?? throw new ArgumentException("Variable Name could not be determined for node: " + this);
            //NodePath = this.GetNodePath(Node.OfKind(SyntaxKind.VariableDeclaration).First());
        }

        public override string GetIdentifier(Node node)
        {
            return node.OfKind(SyntaxKind.VariableDeclaration).First().IdentifierStr;
        }

        public T GetAssignedValue<T>()
            where T : TypeScriptNode
        {
            var arrayLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ArrayLiteralExpression);
            if (arrayLiteral != null)
            {
                return new TypeScriptArrayLiteralExpression(arrayLiteral, this) as T;
            }
            var objectLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ObjectLiteralExpression);
            if (objectLiteral != null)
            {
                return new TypeScriptObjectLiteralExpression(objectLiteral, this) as T;
            }
            // TODO: ValueLiteral

            return null;
        }

        //internal override void UpdateNode()
        //{
        //    Node = (Node)FindNode(NodePath).Parent.Parent;
        //}

        //public bool Equals(TypeScriptVariableStatement other)
        //{
        //    return Name == other?.Name;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((TypeScriptVariableStatement)obj);
        //}

        //public override int GetHashCode()
        //{
        //    return Name.GetHashCode();
        //}
    }
}