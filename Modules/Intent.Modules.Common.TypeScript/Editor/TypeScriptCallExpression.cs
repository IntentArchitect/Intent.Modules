using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptCallExpression : TypeScriptNode
    {
        public TypeScriptCallExpression(Node node, TypeScriptNode parent) : base(node, parent)
        {
            //Identifier =  ?? throw new ArgumentException("Variable identifier could not be determined for node: " + this);
            //NodePath += $"/PropertyAccessExpression~{Identifier}";
        }

        public override string GetIdentifier(Node node)
        {
            return (node.OfKind(SyntaxKind.PropertyAccessExpression).FirstOrDefault()
                    ?? node.OfKind(SyntaxKind.CallExpression).First()).GetText();
        }
    }
}