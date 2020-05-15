using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    //public class TypescriptObjectLiteral : TypescriptNode
    //{
    //    public TypescriptObjectLiteral(Node node, TypescriptFile file) : base(node, file)
    //    {
    //    }

    //    public bool PropertyAssignmentExists(string propertyName)
    //    {
    //        var properties = Node.Children.Where(x => x.Kind == SyntaxKind.PropertyAssignment);

    //        return properties.Any(x => x.IdentifierStr == propertyName);
    //    }

    //    public bool PropertyAssignmentExists(string propertyName, string valueLiteral)
    //    {
    //        var properties = Node.Children.Where(x => x.Kind == SyntaxKind.PropertyAssignment);

    //        var property = properties.FirstOrDefault(x => x.IdentifierStr == propertyName);
    //        if (property == null)
    //        {
    //            return false;
    //        }
    //        return property.Children[1].IdentifierStr == valueLiteral || property.Children[1].GetText() == valueLiteral;
    //    }

    //    public void AddPropertyAssignment(string propertyAssignment)
    //    {
    //        var assignments = Node.Children.Where(x => x.Kind == SyntaxKind.PropertyAssignment);

    //        if (assignments.Any())
    //        {
    //            Change.InsertAfter(assignments.Last(), propertyAssignment);
    //        }
    //        else
    //        {
    //            Change.InsertBefore(Node.Children.Last(), propertyAssignment);
    //        }
    //    }
    //}

    public class TypescriptVariableDeclaration : TypescriptNode
    {
        public TypescriptVariableDeclaration(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public string Name => Node.IdentifierStr;

        public T GetAssignedValue<T>()
            where T : TypescriptNode
        {
            var arrayLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ArrayLiteralExpression);
            if (arrayLiteral != null)
            {
                return new TypescriptArrayLiteralExpression(arrayLiteral, File) as T;
            }
            var objectLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ObjectLiteralExpression);
            if (objectLiteral != null)
            {
                return new TypescriptObjectLiteralExpression(objectLiteral, File) as T;
            }
            // TODO: ValueLiteral

            return null;
        }
    }
}