using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptFile
    {
        private string _source;
        public TypeScriptAST Ast;
        public ChangeAST Change;

        public TypescriptFile(string source)
        {
            _source = source;
            Ast = new TypeScriptAST(_source);
            Change = new ChangeAST();
        }

        public void AddImportIfNotExists(string className, string location)
        {
            var imports = Ast.OfKind(SyntaxKind.ImportDeclaration);

            if (imports.Any())
            {
                if (imports.All(x => x.GetDescendants(false).OfKind(SyntaxKind.Identifier).FirstOrDefault()?.IdentifierStr != className))
                {
                    Change.InsertAfter(imports.Last(), $@"
import {{ {className} }} from '{location}';");
                }
            }
            else
            {
                Change.InsertBefore(Ast.RootNode, $@"
import {{ {className} }} from '{location}';");
            }
        }

        public IList<TypescriptClass> ClassDeclarations()
        {
            return new TypeScriptAST(_source).OfKind(SyntaxKind.ClassDeclaration).Select(x => new TypescriptClass(x, this)).ToList();
        }

        public IList<TypescriptVariableDeclaration> VariableDeclarations()
        {
            return new TypeScriptAST(_source).OfKind(SyntaxKind.VariableDeclaration).Select(x => new TypescriptVariableDeclaration(x, this)).ToList();
        }

        public string GetChangedSource()
        {
            UpdateChanges();
            return _source;
        }

        public void UpdateChanges()
        {
            _source = Change.GetChangedSource(_source);
            Ast = new TypeScriptAST(_source);
            Change = new ChangeAST();
        }
    }
}