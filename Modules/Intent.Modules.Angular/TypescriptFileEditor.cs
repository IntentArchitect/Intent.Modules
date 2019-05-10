using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular
{
    public class TypescriptFileEditor
    {
        private string _source;

        public TypescriptFileEditor(string source)
        {
            _source = source;
        }

        public void AddImportIfNotExists(string className, string location)
        {
            var ast = new TypeScriptAST(_source);
            var change = new ChangeAST();
            var imports = ast.OfKind(SyntaxKind.ImportDeclaration);

            if (imports.Any())
            {
                if (imports.All(x => x.GetDescendants(false).OfKind(SyntaxKind.Identifier).FirstOrDefault()?.IdentifierStr != className))
                {
                    change.InsertAfter(imports.Last(), $@"
import {{ {className} }} from '{location}';");
                }
            }
            else
            {
                change.InsertBefore(ast.RootNode, $@"
import {{ {className} }} from '{location}';");
            }
            _source = change.GetChangedSource(_source);
        }

        public bool MethodExists(string methodName)
        {
            var ast = new TypeScriptAST(_source);
            var methods = ast.OfKind(SyntaxKind.MethodDeclaration);

            if (!methods.Any())
            {
                return false;
            }

            return NodeExists($"MethodDeclaration/Identifier:{methodName}");
        }

        public void AddMethod(string method)
        {
            var ast = new TypeScriptAST(_source);
            var change = new ChangeAST();
            var methods = ast.OfKind(SyntaxKind.MethodDeclaration);

            if (methods.Any())
            {
                change.InsertAfter(methods.Last(), method);
            }
            else
            {
                var classDeclaration = ast.OfKind(SyntaxKind.ClassDeclaration).First();
                change.InsertAfter(classDeclaration.Children.Last(), method);
            }
            _source = change.GetChangedSource(_source);
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
                _source = change.GetChangedSource(_source);
            }
        }

        public void AddProperty(string propertyDeclaration)
        {
            var ast = new TypeScriptAST(_source);
            var change = new ChangeAST();
            var properties = ast.OfKind(SyntaxKind.PropertyDeclaration);

            if (properties.Any())
            {
                change.InsertAfter(properties.Last(), propertyDeclaration);
            }
            else
            {
                var classDeclaration = ast.OfKind(SyntaxKind.ClassDeclaration).First();
                change.InsertAfter(classDeclaration.Children.First(), propertyDeclaration);
            }
            _source = change.GetChangedSource(_source);
        }

        public bool NodeExists(string path)
        {
            return FindNode(path) != null;
        }

        public Node FindNode(string path)
        {
            var ast = new TypeScriptAST(_source);
            return FindNode(ast.RootNode, path);
        }

        public Node FindNode(Node node, string path)
        {
            var pathParts = path.Split('/');
            var part = pathParts[0];

            var syntaxKindValue = part.Split(':')[0];
            var identifier = part.Split(':').Length > 1 ? part.Split(':')[1] : null;

            if (node == null || !Enum.TryParse(syntaxKindValue, out SyntaxKind syntaxKind))
                return null;

            if (pathParts.Length == 1)
            {
                return node.GetDescendants().OfKind(syntaxKind).FirstOrDefault(x => identifier == null || x.IdentifierStr == identifier);
            }

            if (identifier == null)
            {
                foreach (var descendant in node.GetDescendants().OfKind(syntaxKind))
                {
                    var found = FindNode(descendant, path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1));
                    if (found != null)
                    {
                        return found;
                    }
                }

                return null;
            }

            return FindNode(node.GetDescendants().OfKind(syntaxKind).FirstOrDefault(x => x.IdentifierStr == identifier), path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1));
        }

        public void AddProviderIfNotExists(string className)
        {
            var change = new ChangeAST();
            var providers = FindNode("Decorator/CallExpression:NgModule/PropertyAssignment:providers/ArrayLiteralExpression");
            if (providers != null)
            {
                if (providers.Children.Count == 0)
                {
                    change.ChangeNode(providers, $@" [
    {className}
  ]");
                }
                else if (providers.Children.All(x => x.IdentifierStr != className))
                {
                    change.InsertAfter(providers.Children.Last(), $@", 
    {className}");
                }
            }
            _source = change.GetChangedSource(_source);
        }

        public void AddDeclarationIfNotExists(string className)
        {
            var change = new ChangeAST();
            var declarations = FindNode("Decorator/CallExpression:NgModule/PropertyAssignment:declarations/ArrayLiteralExpression");
            if (declarations != null)
            {
                if (declarations.Children.Count == 0)
                {
                    change.ChangeNode(declarations, $@" [
    {className}
  ]");
                }
                else if (declarations.Children.All(x => x.IdentifierStr != className))
                {
                    change.InsertAfter(declarations.Children.Last(), $@", 
    {className}");
                }
            }
            _source = change.GetChangedSource(_source);
        }

        public string GetSource()
        {
            return _source;
        }
    }
}