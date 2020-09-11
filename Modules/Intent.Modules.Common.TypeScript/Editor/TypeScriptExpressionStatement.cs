using System;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptExpressionStatement : TypeScriptNode
    {
        public TypeScriptExpressionStatement(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
            //Identifier =  ?? throw new ArgumentException("Variable identifier could not be determined for node: " + this);
            //NodePath += $"/PropertyAccessExpression~{Identifier}";
        }

        public override string GetIdentifier(Node node)
        {
            return (node.OfKind(SyntaxKind.PropertyAccessExpression).FirstOrDefault()
                ?? node.OfKind(SyntaxKind.CallExpression).First()).GetText();
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

        //    Node = (Node)FindNode(File.Ast.RootNode, NodePath).Parent.Parent;

        //}


        //public bool Equals(TypeScriptExpressionStatement other)

        //{

        //    return Identifier == other?.Identifier;

        //}


        //public override bool Equals(object obj)

        //{

        //    if (ReferenceEquals(null, obj)) return false;

        //    if (ReferenceEquals(this, obj)) return true;

        //    if (obj.GetType() != this.GetType()) return false;

        //    return Equals((TypeScriptExpressionStatement)obj);

        //}


        //public override int GetHashCode()

        //{

        //    return Identifier.GetHashCode();

        //}
    }
}