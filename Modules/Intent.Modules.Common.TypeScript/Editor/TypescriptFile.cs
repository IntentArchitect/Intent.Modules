using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFile
    {
        private string _source;
        public TypeScriptAST Ast;
        public ChangeAST Change;

        public TypeScriptFile(string source)
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
            UpdateChanges();
        }

        public IList<TypeScriptClass> ClassDeclarations()
        {
            return new TypeScriptAST(_source).OfKind(SyntaxKind.ClassDeclaration).Select(x => new TypeScriptClass(x, this)).ToList();
        }

        public IList<TypeScriptVariableDeclaration> VariableDeclarations()
        {
            return new TypeScriptAST(_source).OfKind(SyntaxKind.VariableDeclaration).Select(x => new TypeScriptVariableDeclaration(x, this)).ToList();
        }

        public string GetSource()
        {
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