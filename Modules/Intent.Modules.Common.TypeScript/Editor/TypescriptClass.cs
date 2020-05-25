using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptClass : TypeScriptNode
    {
        public TypeScriptClass(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public string Name => Node.IdentifierStr;

        public IList<TypeScriptClassDecorator> Decorators()
        {
            return Node.OfKind(SyntaxKind.Decorator).Select(x => new TypeScriptClassDecorator(x, File)).ToList();
        }

        public bool MethodExists(string methodName)
        {
            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);

            if (!methods.Any())
            {
                return false;
            }

            return NodeExists($"MethodDeclaration/MethodDeclaration:{methodName}");
        }

        public void AddMethod(string method)
        {
            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);

            if (methods.Any())
            {
                Change.InsertAfter(methods.Last(), method);
            }
            else
            {
                Change.InsertAfter(Node.Children.Last(), method);
            }

            UpdateChanges();
        }

        public void ReplaceMethod(string methodName, string method)
        {
            var existing = FindNode($"ClassDeclaration/MethodDeclaration:{methodName}");

            if (existing == null)
            {
                throw new InvalidOperationException($"Method ({methodName}) could not be found.");
            }

            if (existing.GetTextWithComments() != method)
            {
                Change.ChangeNode(existing, method);
            }

            UpdateChanges();
        }

        public void AddProperty(string propertyDeclaration)
        {
            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);

            if (properties.Any())
            {
                Change.InsertAfter(properties.Last(), propertyDeclaration);
            }
            else
            {
                Change.InsertBefore(Node.Children.OfKind(SyntaxKind.Constructor).FirstOrDefault() ?? Node.Children.OfKind(SyntaxKind.MethodDeclaration).FirstOrDefault() ?? Node.Children.Last(), propertyDeclaration);
            }

            UpdateChanges();
        }

        public bool PropertyExists(string propertyName)
        {
            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);

            return properties.Any(x => x.IdentifierStr == propertyName);
        }
    }
}