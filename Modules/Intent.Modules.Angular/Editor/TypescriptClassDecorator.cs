using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptClassDecorator : TypescriptNode
    {
        public TypescriptClassDecorator(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public string Name => Node.First.IdentifierStr;

        public IEnumerable<TypescriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypescriptObjectLiteralExpression(x, File));
        }
    }

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
                change.InsertAfter(afterNode, propertyAssignment);
            }
            else
            {
                change.InsertBefore(Node.Children.Last(), propertyAssignment);
            }
            File.UpdateChanges(change);
        }

        public void AddPropertyAssignment(string propertyAssignment)
        {
            InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
        }
    }
}