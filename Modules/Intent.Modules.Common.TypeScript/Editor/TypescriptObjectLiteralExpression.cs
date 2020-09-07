using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptObjectLiteralExpression : TypeScriptNode
    {
        public TypeScriptObjectLiteralExpression(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
        }

        public override string GetIdentifier(Node node)
        {
            return Node.GetText(); // No idea...
        }

        public bool PropertyAssignmentExists(string propertyName, string valueLiteral = null)
        {
            var properties = Node.Children.Where(x => x.Kind == SyntaxKind.PropertyAssignment);

            var property = properties.FirstOrDefault(x => x.IdentifierStr == propertyName);
            if (property == null)
            {
                return false;
            }
            return valueLiteral == null || property.Children[1].IdentifierStr == valueLiteral || property.Children[1].GetText() == valueLiteral;
        }

        public void InsertPropertyAssignment(string propertyAssignment, Node afterNode)
        {
            if (afterNode != null)
            {
                if (afterNode.Kind == SyntaxKind.PropertyAssignment)
                {
                    propertyAssignment = $", {propertyAssignment}";
                }
                Editor.Change.InsertAfter(afterNode, propertyAssignment);
            }
            else
            {
                if (Node.Children.OfKind(SyntaxKind.PropertyAssignment).Any())
                {
                    InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
                    return;
                }
                Editor.Change.InsertBefore(Node.Children.Last(), propertyAssignment);
            }
            Editor.UpdateNodes();
        }

        public void AddPropertyAssignment(string propertyAssignment)
        {
            InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
        }
    }
}