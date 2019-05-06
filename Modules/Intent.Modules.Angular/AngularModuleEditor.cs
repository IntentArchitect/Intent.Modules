using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular
{
    public class AngularModuleEditor
    {
        private string _source;

        public AngularModuleEditor(string source)
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

            return methods.Any(x => x.Children.OfKind(SyntaxKind.Identifier).Any(i => i.IdentifierStr == methodName));
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

        public void AddDeclarationIfNotExists(string className)
        {
            var ast = new TypeScriptAST(_source);
            var change = new ChangeAST();
            var declarations = ast.OfKind(SyntaxKind.Decorator).FirstOrDefault(x => x.First.IdentifierStr == "NgModule")?.First
                ?.GetDescendants(false).OfKind(SyntaxKind.PropertyAssignment).FirstOrDefault(x => x.IdentifierStr == "declarations")
                ?.Children[1]; ;
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