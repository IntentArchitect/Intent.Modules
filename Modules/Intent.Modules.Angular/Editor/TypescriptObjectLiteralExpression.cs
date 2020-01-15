using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptObjectLiteralExpression : TypescriptNode
    {
        public TypescriptObjectLiteralExpression(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public bool PropertyAssignmentExists(string propertyName)
        {
            var properties = Node.OfKind(SyntaxKind.PropertyAssignment);

            return properties.Any(x => x.IdentifierStr == propertyName);
        }

        public void InsertPropertyAssignment(string propertyAssignment, Node afterNode)
        {
            if (afterNode != null)
            {
                if (afterNode.Kind == SyntaxKind.PropertyAssignment)
                {
                    propertyAssignment = $", {propertyAssignment}";
                }
                Change.InsertAfter(afterNode, propertyAssignment);
            }
            else
            {
                if (Node.Children.OfKind(SyntaxKind.PropertyAssignment).Any())
                {
                    InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
                    return;
                }
                Change.InsertBefore(Node.Children.Last(), propertyAssignment);
            }
        }

        public void AddPropertyAssignment(string propertyAssignment)
        {
            InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
        }
    }
}