using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptClass : TypescriptNode
    {
        public TypescriptClass(Node node, TypescriptFile file) : base(node, file)
        {

        }

        public string Name => Node.IdentifierStr;

        public IList<TypescriptClassDecorator> Decorators()
        {
            return Node.OfKind(SyntaxKind.Decorator).Select(x => new TypescriptClassDecorator(x, File)).ToList();
        }

        public bool MethodExists(string methodName)
        {
            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);

            if (!methods.Any())
            {
                return false;
            }

            return NodeExists($"MethodDeclaration/Identifier:{methodName}");
        }

        public void AddMethod(string method)
        {
            var change = new ChangeAST();
            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);

            if (methods.Any())
            {
                change.InsertAfter(methods.Last(), method);
            }
            else
            {
                change.InsertAfter(Node.Children.Last(), method);
            }

            File.UpdateChanges(change);
        }

        public void ReplaceMethod(string methodName, string method)
        {
            var change = new ChangeAST();

            var existing = FindNode($"ClassDeclaration/MethodDeclaration:{methodName}");

            if (existing == null)
            {
                throw new InvalidOperationException($"Method ({methodName}) could not be found.");
            }

            if (existing.GetTextWithComments() != method)
            {
                change.ChangeNode(existing, method);
                File.UpdateChanges(change);
            }
        }

        public void AddProperty(string propertyDeclaration)
        {
            var change = new ChangeAST();
            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);

            if (properties.Any())
            {
                change.InsertAfter(properties.Last(), propertyDeclaration);
            }
            else
            {
                change.InsertBefore(Node.Children.OfKind(SyntaxKind.Constructor).FirstOrDefault() ?? Node.Children.OfKind(SyntaxKind.MethodDeclaration).FirstOrDefault() ?? Node.Children.Last(), propertyDeclaration);
            }
            File.UpdateChanges(change);
        }

        public bool PropertyExists(string propertyName)
        {
            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);

            return properties.Any(x => x.IdentifierStr == propertyName);
        }
    }

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