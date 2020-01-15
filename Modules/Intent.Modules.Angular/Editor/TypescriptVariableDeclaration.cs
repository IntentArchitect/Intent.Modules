using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptVariableDeclaration : TypescriptNode
    {
        public TypescriptVariableDeclaration(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public bool PropertyAssignmentExists(string propertyName)
        {
            var properties = Node.OfKind(SyntaxKind.PropertyAssignment);

            return properties.Any(x => x.IdentifierStr == propertyName);
        }

        public void AddPropertyAssignment(string propertyAssignment)
        {
            var change = new ChangeAST();
            var assignments = Node.OfKind(SyntaxKind.PropertyAssignment);

            if (assignments.Any())
            {
                change.InsertAfter(assignments.Last(), propertyAssignment);
            }
            else
            {
                change.InsertBefore(Node.Children.Last(), propertyAssignment);
            }
            File.UpdateChanges(change);
        }
    }
}